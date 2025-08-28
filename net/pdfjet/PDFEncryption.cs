using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
public class PDFEncryption {
    private readonly byte[] key;   // 128-bit AES key
    private readonly byte[] iv;    // 128-bit IV
    private readonly int objNumber;

    /// <summary>
    /// Creates a new AES-128 encryption dictionary and adds it to the PDF.
    /// </summary>
    /// <param name="pdf">The parent PDF document.</param>
    /// <param name="userPassword">The user password string.</param>
    /// <param name="ownerPassword">The owner password string.</param>
    public PDFEncryption(PDF pdf, string userPassword, string ownerPassword) {
        // For AES-256, this is the NEW way (correct)
        // Convert the password strings to UTF-8 bytes directly
        byte[] userPassBytes = Encoding.UTF8.GetBytes(userPassword ?? "");
        byte[] ownerPassBytes = Encoding.UTF8.GetBytes(ownerPassword ?? "");

        // === Generate a random 256-bit (32-byte) file encryption key ===
        this.key = new byte[32]; // 32 bytes for AES-256
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(this.key); // Fills the array with cryptographically strong random bytes
        }

        // === Generate random IV (AES block size = 128-bit) ===
        this.iv = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(this.iv);
        }

        // === Write Encryption Dictionary ===
        pdf.NewObj();
        pdf.Append("<<\n");
        pdf.Append("/Filter /Standard\n");
        pdf.Append("/V 5\n");            // Algorithm 2.A / 2.B
        pdf.Append("/R 6\n");            // Security revision 6 (strong password hashing)
        pdf.Append("/Length 128\n");     // !! Vestigial, required, ignored (must still be present) !!
        pdf.Append("/CF << /StdCF <<\n");
        pdf.Append("  /CFM /AESV3\n");   // AESV3 = AES-256 in CBC
        pdf.Append("  /Length 32\n");    // 32 bytes = 256-bit file key
        pdf.Append("  /AuthEvent /DocOpen\n");
        pdf.Append(">> >>\n");
        pdf.Append("/StmF /StdCF\n");
        pdf.Append("/StrF /StdCF\n");

        // 1. Calculate the values first, storing them in well-named variables
        byte[] ownerPasswordValidationHash = new byte[32]; //ComputeOwnerPasswordHash(ownerPassword, userPassword, permissionFlags);
        byte[] userPasswordValidationHash = new byte[32]; //= ComputeUserPasswordHash(userPassword, permissionFlags);
        byte[] ownerEncryptionKey = new byte[32]; //ComputeEncryptedFileKey(ownerPassword, userPassword, permissionFlags);
        byte[] userEncryptionKey = new byte[32]; //ComputeEncryptedFileKey(userPassword, userPassword, permissionFlags);

        // === Owner key (O) ===
        // < 32-byte-hash 8-byte-validation-salt 8-byte-key-salt >
        // 48 bytes long if the value of R is 6, based on both the owner and user passwords,
        // that shall be used in computing the file encryption key and in
        // determining whether a valid owner password was entered.
        pdf.Append("/O <");
        pdf.Append(ToHex(ownerPasswordValidationHash));
        pdf.Append(">\n");

        // === User key (U) ===
        // < 32-byte-hash 8-byte-validation-salt 8-byte-key-salt >
        // 48 bytes long if the value of R is 6, based on both the owner and user passwords,
        // that shall be used in determining whether to prompt the user for a password
        // and, if so, whether a valid user or owner password was entered.
        pdf.Append("/U <");
        pdf.Append(ToHex(userPasswordValidationHash));
        pdf.Append(">\n");

        // === Owner Encryption Key (OE) ===
        pdf.Append("/OE <");
        pdf.Append(ToHex(ownerEncryptionKey));
        pdf.Append(">\n");

        // === User Encryption Key (UE) ===
        pdf.Append("/UE<");
        pdf.Append(ToHex(userEncryptionKey));
        pdf.Append(">\n");

        // A set of flags specifying which operations shall be permitted
        // when the document is opened with user access (see "Table 22 — User access permissions").
        pdf.Append("/P -3904\n");

        // A 16-byte string, encrypted with the file encryption key,
        // that contains an encrypted copy of the permissions flags.
        // For more information, see 7.6.4.4, "Password algorithms".
        pdf.Append("/Parms ????\n");

        pdf.Append(">>\n");
        pdf.EndObj();

        objNumber = pdf.GetObjNumber();

        // SECURITY: This is the crucial step. Wipe the padded passwords from memory.
        CryptographicOperations.ZeroMemory(userPassBytes);
        CryptographicOperations.ZeroMemory(ownerPassBytes);
    }

    public int GetObjNumber() {
        return objNumber;
    }

    private byte[] ComputeHashValue(byte[] inputPassword, bool isOwnerPassword, byte[] userKey) {
        // Take the SHA-256 hash of the original input to the algorithm and name the resulting 32 bytes, K.
        int round = 0;
        bool continueProcessing = true;
        byte[] K = HashPassword(inputPassword);
        // Calculate the size of K1 *once*, outside the loop
        int k1Size;
        if (isOwnerPassword) {
            // Add a validation check for the user key
            if (userKey == null || userKey.Length != 48) {
                throw new ArgumentException(
                    "User key must be provided and be 48 bytes long for owner password verification.", nameof(userKey));
            }
            k1Size = 64 * (inputPassword.Length + K.Length + userKey.Length);
        } else {
            k1Size = 64 * (inputPassword.Length + K.Length);
        }
        byte[] K1;
        byte[] E;
        using (MemoryStream stream = new MemoryStream(k1Size)) {
            // Perform the following steps (a)-(d) 64 times:
            while (round < 64 || continueProcessing) {
                // a) Make a new string, K1, consisting of 64 repetitions of the sequence:
                //    input password, K, the 48-byte user key.
                //    The 48 byte user key is only used when checking the owner password or creating the owner key.
                //    If checking the user password or creating the user key,
                //    K1 is the concatenation of the input password and K.
                stream.Position = 0; // Reset the stream
                for (int j = 0; j < 64; j++) {
                    if (isOwnerPassword) {
                        stream.Write(inputPassword, 0, inputPassword.Length);
                        stream.Write(K, 0, K.Length);
                        stream.Write(userKey, 0, userKey.Length);   // The 48-byte user key
                    } else {    // user password
                        stream.Write(inputPassword, 0, inputPassword.Length);
                        stream.Write(K, 0, K.Length);
                    }
                }
                K1 = stream.ToArray();

                // b) Encrypt K1 with the AES-128 (CBC, no padding) algorithm,
                //    using the first 16 bytes of K as the key and the second
                //    16 bytes of K as the initialization vector.
                //    The result of this encryption is E.
                byte[] tempKey = new byte[16];
                Array.Copy(K, 0, tempKey, 0, 16);
                byte[] tempIV = new byte[16];
                Array.Copy(K, 16, tempIV, 0, 16);
                E = EncryptAlgorithmStep2B(K1, tempKey, tempIV);

                // --- Steps (c) & (d): Common to all rounds ---
                // c) Taking the first 16 bytes of E as an unsigned big-endian integer...
                // d) Using the hash algorithm determined in step c, take the hash of E.
                using (HashAlgorithm hashAlgo = DetermineNextHashAlgorithm(E)) {
                    K = hashAlgo.ComputeHash(E);
                }

                // --- Steps (e) & (f): The Termination Check (For rounds 64+ only) ---
                // Following 64 rounds (round number 0 to round number 63),
                // do the following, starting with round number 64:
                if (round >= 64) {
                    // e) Look at the very last byte of E.
                    //    If the value of that byte (taken as an unsigned integer)
                    //    is greater than the round number - 32, repeat steps (a-d) again.
                    byte lastByte = E[E.Length - 1];
                    continueProcessing = (lastByte > (round - 32));
                }
                // f) Repeat from steps (a-e) until the value of the last byte is ≤ (round number) - 32.

                round++; // Increment the round counter
            }
        }
        // NOTE 3
        // Tests indicate that the total number of rounds will most likely be between 65 and 80.
        // !! We can print this number to verify we are in this range !!
        Console.WriteLine("Number of rounds: " + round);

        byte[] finalOutput = new byte[32];
        Array.Copy(K, 0, finalOutput, 0, 32);
        return finalOutput;
    }

    /// <summary>
    /// Performs the encryption for Step (b) of Algorithm 2.B.
    /// Encrypts the input data using AES-128-CBC with no padding, using the provided key and IV.
    /// </summary>
    /// <param name="data">The data to encrypt (the K1 array).</param>
    /// <param name="key">The 16-byte AES key.</param>
    /// <param name="iv">The 16-byte initialization vector.</param>
    /// <returns>The ciphertext result E.</returns>
    private byte[] EncryptAlgorithmStep2B(byte[] data, byte[] key, byte[] iv) {
        // Input validation
        if (key.Length != 16) throw new ArgumentException("Key must be 16 bytes for AES-128.", nameof(key));
        if (iv.Length != 16) throw new ArgumentException("IV must be 16 bytes.", nameof(iv));

        using (Aes aes = Aes.Create()) {
            // Configure EXACTLY as specified for Algorithm 2.B, Step (b)
            aes.KeySize = 128;              // Must be AES-128
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;      // Must be CBC mode
            aes.Padding = PaddingMode.None; // CRITICAL: No padding

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                // Encrypt the entire data array without padding
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();       // Still necessary to process all data
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Analyzes the first 16 bytes of the ciphertext 'E' to determine the next hash algorithm to use.
    /// </summary>
    /// <param name="ciphertextE">The ciphertext output from the encryption step.</param>
    /// <returns>An instance of the chosen hash algorithm (SHA256, SHA384, or SHA512).</returns>
    private HashAlgorithm DetermineNextHashAlgorithm(byte[] ciphertextE) {
        if (ciphertextE.Length < 16) {
            throw new ArgumentException("The input array must be at least 16 bytes long.", nameof(ciphertextE));
        }

        // 1. Take the first 16 bytes of E and convert to an unsigned big-endian integer
        BigInteger bigInt = new BigInteger(
            new ReadOnlySpan<byte>(ciphertextE, 0, 16),
                isUnsigned: true,
                isBigEndian: true);

        // 2. Compute the remainder, modulo 3
        int remainder = (int)(bigInt % 3);

        // 3. Return the right hash algorithm
        return remainder switch {
            0 => SHA256.Create(),
            1 => SHA384.Create(),
            2 => SHA512.Create(),
            _ => throw new InvalidOperationException() // Required by the compiler
        };
    }

    /// <summary>
    /// Encrypts data with AES-128-CBC or AES-256-CBC and PKCS#7 padding.
    /// </summary>
    public byte[] Encrypt(byte[] plain, int keySize) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = keySize;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                cs.Write(plain, 0, plain.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    // === Helpers ===
    private byte[] HashPassword(byte[] input) {
        using (SHA256 sha256 = SHA256.Create()) {
            return sha256.ComputeHash(input);
        }
    }

    private string ToHex(byte[] bytes) {
        char[] hex = new char[bytes.Length * 2];
        const string HEX_CHARS = "0123456789ABCDEF";
        for (int i = 0; i < bytes.Length; i++) {
            hex[i * 2]     = HEX_CHARS[bytes[i] >> 4];
            hex[i * 2 + 1] = HEX_CHARS[bytes[i] & 0x0F];
        }
        return new string(hex);
    }
}
}
