using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
internal sealed class UserPair {
    public byte[] U;
    public byte[] UE;
    public UserPair(byte[] U, byte[] UE) {
        this.U = U;
        this.UE = UE;
    }
}

internal sealed class OwnerPair {
    public byte[] O;
    public byte[] OE;
    public OwnerPair(byte[] O, byte[] OE) {
        this.O = O;
        this.OE = OE;
    }
}

public class PDFEncryption {
    private readonly int objNumber;
    private readonly byte[] fileEncryptionKey;

    /// <summary>
    /// Creates a new AES-128 encryption dictionary and adds it to the PDF.
    /// </summary>
    /// <param name="pdf">The parent PDF document.</param>
    /// <param name="userPassword">The user password string.</param>
    /// <param name="ownerPassword">The owner password string.</param>
    public PDFEncryption(PDF pdf, string userPassword, string ownerPassword) {
        // === Generate a random 256-bit (32-byte) file encryption key ===
        this.fileEncryptionKey = new byte[32]; // 32 bytes for AES-256
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(this.fileEncryptionKey); // Fills the array with cryptographically strong random bytes
        }

        UserPair userPair = ComputeUserPair(userPassword, fileEncryptionKey);
        OwnerPair ownerPair = ComputeOwnerPair(ownerPassword, userPair.U, fileEncryptionKey);

        // === Encryption Dictionary ===
        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);
        pdf.Append("/Filter /Standard\n");
        pdf.Append("/V 5\n");           // Algorithm 2.A / 2.B
        pdf.Append("/R 6\n");           // Security revision 6 (strong password hashing)
        pdf.Append("/Length 256\n");    // Vestigial, required, ignored (must still be present)
        pdf.Append("/CF <<\n");
        pdf.Append("/StdCF <<\n");
        pdf.Append("/CFM /AESV3\n");    // AESV3 = AES-256 in CBC
        pdf.Append("/Length 32\n");     // 32 bytes = 256-bit file key
        pdf.Append("/AuthEvent /DocOpen\n");
        pdf.Append(">>\n");
        pdf.Append(">>\n");
        pdf.Append("/StmF /StdCF\n");
        pdf.Append("/StrF /StdCF\n");

        pdf.Append("/U <");             // === User Key (U) ===
        pdf.Append(ToHex(userPair.U));
        pdf.Append(">\n");

        pdf.Append("/O <");             // === Owner Key (O) ===
        pdf.Append(ToHex(ownerPair.O));
        pdf.Append(">\n");

        pdf.Append("/UE <");            // === User Encryption Key (UE) ===
        pdf.Append(ToHex(userPair.UE));
        pdf.Append(">\n");

        pdf.Append("/OE <");            // === Owner Encryption Key (OE) ===
        pdf.Append(ToHex(ownerPair.OE));
        pdf.Append(">\n");

        // A set of flags specifying which operations shall be permitted
        // when the document is opened with user access (see "Table 22 — User access permissions").
        pdf.Append("/P 4294967292\n");

        // A 16-byte string, encrypted with the file encryption key,
        // that contains an encrypted copy of the permissions flags.
        // For more information, see 7.6.4.4, "Password algorithms".
        pdf.Append("/Perms <065497aaca85a677d5669f0cb68f2cd7>\n");    // TODO:

        pdf.Append(Token.EndDictionary);
        pdf.EndObj();

        objNumber = pdf.GetObjNumber();

        // SECURITY: This is the crucial step. Wipe the padded passwords from memory.
        // CryptographicOperations.ZeroMemory(userPassBytes);
        // CryptographicOperations.ZeroMemory(ownerPassBytes);
    }

    public int GetObjNumber() {
        return objNumber;
    }

    /// <summary>
    /// Computes a hash value based on the provided password and an optional user key.
    /// This method performs the core algorithm for key derivation, iterating 64 times (or more)
    /// to generate a 32-byte hash as a final output. The input password is combined with the SHA-256
    /// hash of the password and the user password hash (if provided) to create a complex keying material.
    /// </summary>
    /// <param name="inputPassword">
    /// The password input (either user or owner password) to be hashed.
    /// </param>
    /// <param name="U">
    /// The 48-byte hash of the user password, required for verifying or creating the owner key.
    /// This is `null` when hashing the user password.
    /// </param>
    /// <returns>
    /// Returns a 32-byte hash value derived from the input password and the optional user password hash.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the `U` is not exactly 48 bytes long when provided.
    /// </exception>
    private byte[] ComputeHash(
            byte[] inputPassword,
            byte[] U) {
        // Take the SHA-256 hash of the original input to the algorithm and name the resulting 32 bytes, K.
        byte[] K = HashPassword(inputPassword);

        // Calculate the size of K1 once, outside the loop
        int k1Size;
        if (U != null) {
            // Validate the user key
            if (U.Length != 48) {
                throw new ArgumentException(
                    "U must be provided and be 48 bytes long for owner password verification.",
                    nameof(U));
            }
            // Correct size calculation for K1 when U is provided.
            // NOTE: K.Length is initially 32 bytes, however in later rounds
            // could be up to 64 bytes when SHA-512 is used.
            k1Size = 64 * (inputPassword.Length + 64 /* K.Length */ + 48 /* U.Length */);
        } else {
            // Correct size calculation for K1 when no U is provided
            k1Size = 64 * (inputPassword.Length + 64 /* K.Length */);
        }

        using (MemoryStream stream = new MemoryStream(k1Size)) {
            // Perform the following steps (a)-(d) 64 times or more:
            int round = 0;
            bool continueProcessing = true;
            while (round < 64 || continueProcessing) {
                // a) Make a new string, K1, consisting of 64 repetitions of the sequence:
                //    input password, K, the 48-byte user key.
                //    The 48 byte user key is only used when checking the owner password or creating the owner key.
                //    If checking the user password or creating the user key,
                //    K1 is the concatenation of the input password and K.
                byte[] K1 = ComputeK1(stream, inputPassword, K, U);

                // b) Encrypt K1 with the AES-128 (CBC, no padding) algorithm,
                //    using the first 16 bytes of K as the key and the second
                //    16 bytes of K as the initialization vector.
                //    The result of this encryption is E.
                byte[] tempKey = new byte[16];
                Array.Copy(K, 0, tempKey, 0, 16);
                byte[] tempIV = new byte[16];
                Array.Copy(K, 16, tempIV, 0, 16);
                byte[] E = AES.EncryptK1(K1, tempKey, tempIV); // Algorithm 2.B, Step (b)

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
                    // f) Repeat from steps (a-e) until the value of the last byte is ≤ (round number) - 32.
                    continueProcessing = (lastByte > (round - 32));
                }

                round++; // Increment the round counter
            }

            // Tests indicate that the total number of rounds will most likely be between 65 and 80.
            Console.WriteLine("Number of rounds: " + round);
        }

        byte[] finalOutput = new byte[32];
        Array.Copy(K, 0, finalOutput, 0, 32);
        return finalOutput;
    }

    private byte[] ComputeK1(MemoryStream stream, byte[] inputPassword, byte[] K, byte[] U) {
        stream.Position = 0;    // Reset the stream
        for (int i = 0; i < 64; i++) {
            stream.Write(inputPassword, 0, inputPassword.Length);
            stream.Write(K, 0, K.Length);
            if (U != null) {
                stream.Write(U, 0, U.Length);
            }
        }
// Console.WriteLine(stream.Length);
        return stream.ToArray();    // Return K1
    }

    /// <summary>
    /// Analyzes the first 16 bytes of the 'E' to determine the next hash algorithm to use.
    /// </summary>
    /// <param name="E">The output from the encryption step.</param>
    /// <returns>An instance of the chosen hash algorithm (SHA256, SHA384, or SHA512).</returns>
    private HashAlgorithm DetermineNextHashAlgorithm(byte[] E) {
        if (E.Length < 16) {
            throw new ArgumentException("The input array must be at least 16 bytes long.", nameof(E));
        }

        // 1. Take the first 16 bytes of E and convert to an unsigned big-endian integer
        BigInteger bigInt = new BigInteger(
            new ReadOnlySpan<byte>(E, 0, 16),
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

    // === Helpers ===
    private byte[] HashPassword(byte[] input) {
        using (SHA256 sha256 = SHA256.Create()) {
            return sha256.ComputeHash(input);
        }
    }

    private string ToHex(byte[] bytes) {
        char[] hex = new char[bytes.Length * 2];
        const string HEX_CHARS = "0123456789abcdef";
        for (int i = 0; i < bytes.Length; i++) {
            hex[i * 2]     = HEX_CHARS[bytes[i] >> 4];
            hex[i * 2 + 1] = HEX_CHARS[bytes[i] & 0x0F];
        }
        return new string(hex);
    }

    private static byte[] HexStringToByteArray(string hexString) {
        if (string.IsNullOrEmpty(hexString))
            throw new ArgumentException("Hex string cannot be null or empty");

        if (hexString.Length % 2 != 0)
            throw new ArgumentException("Hex string must have an even length");

        int length = hexString.Length;
        byte[] byteArray = new byte[length / 2];

        for (int i = 0; i < length; i += 2) {
            string byteValue = hexString.Substring(i, 2);
            byteArray[i / 2] = Convert.ToByte(byteValue, 16);
        }

        return byteArray;
    }

    //<<
    //    /Filter /AESDecode
    //    /V 5
    //    /R 6
    //    /Length <Encrypted Data Length>
    //    /IV <Base64-encoded IV>
    //    /Data <Encrypted Stream Data>
    //>>
    // Encrypt a stream and store the IV
    public static Dictionary<string, object> EncryptStreamWithIV(byte[] streamData, byte[] key) {
//        // Generate a random 16-byte IV
//        // byte[] iv = RandomNumberGenerator.GetBytes(16);
//
//        // Encrypt the data with AES-256-CBC
//        EncryptedDataWithIV encryptedDataWithIV = AES.EncryptAes256(streamData, key);

        // Store the IV and encrypted data in the stream dictionary
        var streamDict = new Dictionary<string, object>();
//        streamDict["Filter"] = "/AESDecode";
//        streamDict["V"] = 5;  // PDF 2.0 version for AES-256 encryption
//        streamDict["R"] = 6;  // Revision for AES encryption
//        streamDict["Length"] = encryptedDataWithIV.encryptedData.Length;
//        streamDict["IV"] = Convert.ToBase64String(encryptedDataWithIV.IV);  // Store IV as Base64 string
//        streamDict["Data"] = encryptedDataWithIV;            // Store encrypted data

        return streamDict;
    }

    // 7.6.4.4.7
    // Algorithm 8: Computing the encryption dictionary’s U (user password) and
    // UE (user encryption) values (Security handlers of revision 6)
    // a) Generate 16 random bytes of data using a strong random number generator. The first 8 bytes are the
    //    User Validation Salt. The second 8 bytes are the User Key Salt. Compute the 32-byte hash using algorithm
    //    2.B with an input string consisting of the UTF-8 password concatenated with the User Validation Salt.
    //    The 48- byte string consisting of the 32-byte hash followed by the User Validation Salt followed by the
    //    User Key Salt is stored as the U key.
    //
    // b) Compute the 32-byte hash using algorithm 2.B with an input string consisting of the UTF-8 password
    //    concatenated with the User Key Salt. Using this hash as the key, encrypt the file encryption key using
    //    AES-256 in CBC mode with no padding and an initialization vector of zero. The resulting 32-byte string is
    //    stored as the UE key.
    internal UserPair ComputeUserPair(
            String userPassword,
            byte[] fileEncryptionKey) {
        byte[] randomBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomBytes);
        }

        byte[] userPasswordBytes = Encoding.UTF8.GetBytes(userPassword);
        byte[] userValidationSalt = new byte[8];
        byte[] userKeySalt = new byte[8];
        Array.Copy(randomBytes, 0, userValidationSalt, 0, 8);
        Array.Copy(randomBytes, 8, userKeySalt, 0, 8);

        userValidationSalt = HexStringToByteArray("6cab48290d91a5a9");
        userKeySalt = HexStringToByteArray("c150dfd58a44edea");

        byte[] hash = ComputeHash(Concatenate(userPasswordBytes, userValidationSalt), null);
        byte[] U = Concatenate(hash, userValidationSalt, userKeySalt);

        hash = ComputeHash(Concatenate(userPasswordBytes, userKeySalt), null);
        byte[] UE = AES.EncryptKeyWithZeroIV(fileEncryptionKey, hash);

        return new UserPair(U, UE);
    }

    // 7.6.4.4.8
    // Algorithm 9: Computing the encryption dictionary’s O (owner password)
    // and OE (owner encryption) values (Security handlers of revision 6)
    // a) Generate 16 random bytes of data using a strong random number generator. The first 8 bytes are the
    //    Owner Validation Salt. The second 8 bytes are the Owner Key Salt. Compute the 32-byte hash using
    //    algorithm 2.B with an input string consisting of the UTF-8 password concatenated with the Owner
    //    Validation Salt and then concatenated with the 48-byte U string as generated in Algorithm 8. The 48-byte
    //    string consisting of the 32-byte hash followed by the Owner Validation Salt followed by the Owner Key
    //    Salt is stored as the O key.
    // b) Compute the 32-byte hash using 7.6.4.3.3, "Algorithm 2.B: Computing a hash (revision 6 and later)" with
    //    an input string consisting of the UTF-8 password concatenated with the Owner Key Salt and then
    //    concatenated with the 48-byte U string as generated in 7.6.4.4.6, "Algorithm 8: Computing the
    //    encryption dictionary’s U (user password) and UE (user encryption) values (Security handlers of
    //    revision 6)". Using this hash as the key, encrypt the file encryption key using AES-256 in CBC mode with
    //    no padding and an initialization vector of zero. The resulting 32-byte string is stored as the OE key.
    internal OwnerPair ComputeOwnerPair(
            String ownerPassword,
            byte[] U,
            byte[] fileEncryptionKey) {
        byte[] randomBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomBytes);
        }

        byte[] ownerPasswordBytes = Encoding.UTF8.GetBytes(ownerPassword);
        byte[] ownerValidationSalt = new byte[8];
        byte[] ownerKeySalt = new byte[8];
        Array.Copy(randomBytes, 0, ownerValidationSalt, 0, 8);
        Array.Copy(randomBytes, 8, ownerKeySalt, 0, 8);

        byte[] hash = ComputeHash(Concatenate(ownerPasswordBytes, ownerValidationSalt, U), U);
        byte[] O = Concatenate(hash, ownerValidationSalt, ownerKeySalt);

        hash = ComputeHash(Concatenate(ownerPasswordBytes, ownerKeySalt, U), U);
        byte[] OE = AES.EncryptKeyWithZeroIV(fileEncryptionKey, hash);

        return new OwnerPair(O, OE);
    }

    internal byte[] Concatenate(byte[] array1, byte[] array2) {
        // Create a new array with the combined length of both arrays
        byte[] result = new byte[array1.Length + array2.Length];

        // Copy the first array into the result
        Array.Copy(array1, 0, result, 0, array1.Length);

        // Copy the second array into the result, starting after the first array's data
        Array.Copy(array2, 0, result, array1.Length, array2.Length);

        return result;
    }

    internal byte[] Concatenate(byte[] array1, byte[] array2, byte[] array3) {
        // Create a new array with the combined length of both arrays
        byte[] result = new byte[array1.Length + array2.Length + array3.Length];

        // Copy the first array into the result
        Array.Copy(array1, 0, result, 0, array1.Length);

        // Copy the second array into the result, starting after the first array's data
        Array.Copy(array2, 0, result, array1.Length, array2.Length);

        // Copy the third array into the result, starting after the first array's data
        Array.Copy(array3, 0, result, array1.Length + array2.Length, array3.Length);

        return result;
    }
}
}