using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
internal sealed class User {
    public byte[] U;
    public byte[] UE;
    public User(byte[] U, byte[] UE) {
        this.U = U;
        this.UE = UE;
    }
}

internal sealed class Owner {
    public byte[] O;
    public byte[] OE;
    public Owner(byte[] O, byte[] OE) {
        this.O = O;
        this.OE = OE;
    }
}

public class PDFEncryption {
    private readonly int objNumber;
    private readonly byte[] fileEncryptionKey;
    private readonly SHA256 sha256;
    private readonly SHA384 sha384;
    private readonly SHA512 sha512;
    private MemoryStream stream;

    /// <summary>
    /// Creates a new AES-128 encryption dictionary and adds it to the PDF.
    /// </summary>
    /// <param name="pdf">The parent PDF document.</param>
    /// <param name="userPassword">The user password string.</param>
    /// <param name="ownerPassword">The owner password string.</param>
    public PDFEncryption(PDF pdf, string userPassword, string ownerPassword) {
        // === Generate a random 256-bit (32-byte) File Encryption Key ===
        this.fileEncryptionKey = new byte[32]; // 32 bytes for AES-256
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(this.fileEncryptionKey); // Fills the array with cryptographically strong random bytes
        }

        sha256 = SHA256.Create();
        sha384 = SHA384.Create();
        sha512 = SHA512.Create();

        stream = new MemoryStream((int) Math.Pow(2, 15));  // 32 KB buffer
        if (userPassword.Length > 127) {
            userPassword = userPassword.Substring(0, 127);
        }
        if (ownerPassword.Length > 127) {
            ownerPassword = ownerPassword.Substring(0, 127);
        }
        byte[] userPasswordBytes = Encoding.UTF8.GetBytes(userPassword);
        byte[] ownerPasswordBytes = Encoding.UTF8.GetBytes(ownerPassword);

        User user = ComputeUserPair(userPasswordBytes, fileEncryptionKey);
        Owner owner = ComputeOwnerPair(ownerPasswordBytes, user.U, fileEncryptionKey);

        // === Encryption Dictionary ===
        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);
        pdf.Append("/Filter /Standard\n");
        pdf.Append("/V 5\n");           // Algorithm 2.A / 2.B
        pdf.Append("/R 6\n");           // Security revision 6 (strong password hashing)
        // pdf.Append("/Length 256\n");    // Vestigial, required, ignored (must still be present)
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
        pdf.Append(ToHex(user.U));
        pdf.Append(">\n");

        pdf.Append("/O <");             // === Owner Key (O) ===
        pdf.Append(ToHex(owner.O));
        pdf.Append(">\n");

        pdf.Append("/UE <");            // === User Encryption Key (UE) ===
        pdf.Append(ToHex(user.UE));
        pdf.Append(">\n");

        pdf.Append("/OE <");            // === Owner Encryption Key (OE) ===
        pdf.Append(ToHex(owner.OE));
        pdf.Append(">\n");

        // A set of flags specifying which operations shall be permitted
        // when the document is opened with user access (see "Table 22 — User access permissions").
        pdf.Append("/P -3904\n");

        // A 16-byte string, encrypted with the file encryption key,
        // that contains an encrypted copy of the permissions flags.
        // For more information, see 7.6.4.4, "Password algorithms".
        pdf.Append("/Perms <065497aaca85a677d5669f0cb68f2cd7>\n");    // TODO:
        pdf.Append("/EncryptMetadata false\n");

        pdf.Append(Token.EndDictionary);
        pdf.EndObj();

        objNumber = pdf.GetObjNumber();

        stream.Dispose();

        sha256.Dispose();
        sha384.Dispose();
        sha512.Dispose();

        CryptographicOperations.ZeroMemory(userPasswordBytes);
        CryptographicOperations.ZeroMemory(ownerPasswordBytes);
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
            byte[] salt,
            byte[] U) {
        // Take the SHA-256 hash of the original input to the algorithm and name the resulting 32 bytes, K.
        byte[] K = sha256.ComputeHash(Concatenate(inputPassword, salt, U));

        // Perform the following steps (a)-(d) 64 times or more:
        int round = 0;
        while (true) {
            // a) Make a new string, K1, consisting of 64 repetitions of the sequence:
            //    inputPassword, K, U
            byte[] K1 = ComputeK1(inputPassword, K, U);

            // b) Encrypt K1 with the AES-128 (CBC, no padding) algorithm,
            //    using the first 16 bytes of K as the key and the second
            //    16 bytes of K as the initialization vector.
            //    The result of this encryption is E.
            byte[] tempKey = new byte[16];
            Buffer.BlockCopy(K, 0, tempKey, 0, 16);
            byte[] tempIV = new byte[16];
            Buffer.BlockCopy(K, 16, tempIV, 0, 16);
            byte[] E = AES.EncryptK1(K1, tempKey, tempIV); // Algorithm 2.B, Step (b)

            // --- Steps (c) & (d): Common to all rounds ---
            // c) Taking the first 16 bytes of E as an unsigned big-endian integer...
            int algorithm = NextHashAlgorithm(E);
            // d) Using the hash algorithm determined in step c, take the hash of E.
            if (algorithm == 0) {
                K = sha256.ComputeHash(E);
            } else if (algorithm == 1) {
                K = sha384.ComputeHash(E);
            } else if (algorithm == 2) {
                K = sha512.ComputeHash(E);
            }

            // --- Steps (e) & (f): The Termination Check (For rounds 64+ only) ---
            // Following 64 rounds (round number 0 to round number 63),
            // do the following, starting with round number 64:
            if (round >= 64 && E[E.Length - 1] <= (round - 32)) {
                break;
            }

            round++;
        }

        byte[] finalOutput = new byte[32];
        Buffer.BlockCopy(K, 0, finalOutput, 0, 32);
        return finalOutput;
    }

    private byte[] ComputeK1(byte[] inputPassword, byte[] K, byte[] U) {
        stream.SetLength(0);        // Reset the stream
        for (int i = 0; i < 64; i++) {
            stream.Write(inputPassword, 0, inputPassword.Length);
            stream.Write(K, 0, K.Length);
            stream.Write(U, 0, U.Length);
        }
        return stream.ToArray();    // Return K1
    }

    /// <summary>
    /// Analyzes the first 16 bytes of the 'E' to determine the next hash algorithm to use.
    /// </summary>
    /// <param name="E">The output from the encryption step.</param>
    /// <returns>The number of the chosen hash algorithm (0 -> SHA256, 1 -> SHA384, or 2 -> SHA512).</returns>
    private int NextHashAlgorithm(byte[] E) {
        if (E.Length < 16) {
            throw new ArgumentException("The input array must be at least 16 bytes long.", nameof(E));
        }
        // PDF 2.0 specification states:
        // "Taking the first 16 bytes of E as an unsigned big-endian integer, compute the remainder, modulo 3."
        // DeepSeek and Lumo confirm this statement is correct:
        // "The sum of all bytes mod 3 is equivalent to taking the modulo 3 of the whole number."
        int sum = 0;
        for (int i = 0; i < 16; i++) {
            sum += E[i];
        }
        return sum % 3;
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
    internal User ComputeUserPair(
            byte[] userPasswordBytes,
            byte[] fileEncryptionKey) {
        byte[] randomBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomBytes);
        }

        byte[] userValidationSalt = new byte[8];
        byte[] userKeySalt = new byte[8];
        Buffer.BlockCopy(randomBytes, 0, userValidationSalt, 0, 8);
        Buffer.BlockCopy(randomBytes, 8, userKeySalt, 0, 8);

        byte[] hash = ComputeHash(userPasswordBytes, userValidationSalt, new byte[] {});
        byte[] U = Concatenate(hash, userValidationSalt, userKeySalt);

        hash = ComputeHash(userPasswordBytes, userKeySalt, new byte[] {});
        byte[] UE = AES.EncryptKeyWithZeroIV(fileEncryptionKey, hash);

        return new User(U, UE);
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
    internal Owner ComputeOwnerPair(
            byte[] ownerPasswordBytes,
            byte[] U,
            byte[] fileEncryptionKey) {
        byte[] randomBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomBytes);
        }

        byte[] ownerValidationSalt = new byte[8];
        byte[] ownerKeySalt = new byte[8];
        Buffer.BlockCopy(randomBytes, 0, ownerValidationSalt, 0, 8);
        Buffer.BlockCopy(randomBytes, 8, ownerKeySalt, 0, 8);

        byte[] hash = ComputeHash(ownerPasswordBytes, ownerValidationSalt, U);
        byte[] O = Concatenate(hash, ownerValidationSalt, ownerKeySalt);

        hash = ComputeHash(ownerPasswordBytes, ownerKeySalt, U);
        byte[] OE = AES.EncryptKeyWithZeroIV(fileEncryptionKey, hash);

        return new Owner(O, OE);
    }

    internal byte[] Concatenate(byte[] array1, byte[] array2, byte[] array3) {
        // Create a new array with the combined length all three arrays
        byte[] result = new byte[array1.Length + array2.Length + array3.Length];

        // Copy the first array into the result
        Buffer.BlockCopy(array1, 0, result, 0, array1.Length);

        // Copy the second array into the result, starting after the first array's data
        Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);

        // Copy the third array into the result, starting after the first array's data
        Buffer.BlockCopy(array3, 0, result, array1.Length + array2.Length, array3.Length);

        return result;
    }
}   // End of PDFEncryption.cs
}   // End of namespace PDFjet.NET
