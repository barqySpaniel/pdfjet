using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
internal sealed class DerivedKeyWithSalt {
    public byte[] DerivedKey;    // The derived AES key (e.g., 256 bits for AES-256)
    public byte[] Salt;          // The salt used during key derivation

    // Constructor to initialize the data
    public DerivedKeyWithSalt(byte[] derivedKey, byte[] salt) {
        this.DerivedKey = derivedKey;
        this.Salt = salt;
    }
}

internal sealed class EncryptedDataWithIV {
    public byte[] encryptedData;    // The actual encrypted data
    public byte[] iv;               // Initialization vector (IV) used during encryption

    // Constructor to initialize the data
    public EncryptedDataWithIV(byte[] encryptedData, byte[] iv) {
        this.encryptedData = encryptedData;
        this.iv = iv;
    }
}

public class AES {
    /// <summary>
    /// Derives an AES key from a user password using the PBKDF2 key derivation function (KDF).
    /// </summary>
    /// <param name="password">The user-provided password for key derivation.</param>
    /// <param name="keySize">The size of the derived key in bytes (32 bytes for AES-256 by default).</param>
    /// <param name="iterations">The number of PBKDF2 iterations (default 100,000 for stronger security).</param>
    /// <returns>
    /// A DerivedKeyWithSalt object containing the derived key and the salt used for derivation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the password is null.</exception>
    internal static DerivedKeyWithSalt DeriveKeyFromPassword(
            string password,
            int keySize = 32,           // Default is 32 bytes for AES-256
            int iterations = 100000) {  // Stronger iterations count required by the PDF specification

        // Ensure password is provided (to prevent errors)
        if (password == null) {
            throw new ArgumentNullException(nameof(password), "Password cannot be null.");
        }

        // Generate a random salt (e.g., 16 bytes) for each password to ensure unique key derivation each time
        // Salt ensures that the same password results in different derived keys.
        byte[] salt = RandomNumberGenerator.GetBytes(16);  // 16 bytes salt is common for AES-256

        // Create a PBKDF2 key derivation function with the specified password,
        // salt, iterations, and SHA-256 hash algorithm
        using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,                // User password
                salt,                    // Random salt for key derivation
                iterations,              // Number of iterations (higher = more secure but slower)
                HashAlgorithmName.SHA256)) { // SHA-256 hash function for PBKDF2

            // Generate the derived key of the requested size (default is 32 bytes for AES-256)
            // This key is what will be used for encryption or decryption (AES key)
            byte[] derivedKey = pbkdf2.GetBytes(keySize);

            // Return both the derived key and the salt wrapped in the DerivedKeyWithSalt object
            // This ensures that both the key and salt can be stored together and retrieved for decryption
            return new DerivedKeyWithSalt(derivedKey, salt);
        }
    }

    /// <summary>
    /// Encrypts data using AES-128-CBC (per Algorithm 2.B, Step (b)) with no padding.
    /// The provided IV is used for encryption, and only the resulting ciphertext is returned.
    /// </summary>
    /// <param name="data">
    /// The data to encrypt (K1 array in Algorithm 2.B).
    /// </param>
    /// <param name="key">
    /// The 16-byte AES key used for encryption (AES-128).
    /// </param>
    /// <param name="iv">
    /// The 16-byte initialization vector (IV) used for encryption (provided as input).
    /// </param>
    /// <returns>
    /// The encrypted ciphertext as a byte array. IV is not returned.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the data is null or empty, or if the key is not 16 bytes (AES-128).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if encryption fails due to an internal error.
    /// </exception>
    internal static byte[] EncryptAlgorithm2BStepB(byte[] data, byte[] key, byte[] iv) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException(
                "Plaintext data cannot be empty for encryption.", nameof(data));
        }
        if (key == null || key.Length != 16) {
            throw new ArgumentException(
                "Key must be 16 bytes for AES-128-CBC (per Algorithm 2.B).", nameof(key));
        }
        if (iv == null || iv.Length != 16) {  // Ensure IV is exactly 16 bytes
            throw new ArgumentException(
                "IV must be 16 bytes long.", nameof(iv));
        }

        try {
            using (Aes aes = Aes.Create()) {
                // Algorithm 2.B always uses AES-128-CBC to encrypt K1,
                // regardless of the file key length (AES-128 or AES-256)
                aes.KeySize = 128;                      // Use AES-128 (128-bit key)
                aes.Key = key;                          // Set the encryption key
                aes.IV = iv;                            // Set the provided initialization vector (IV)
                aes.Mode = CipherMode.CBC;              // Use CBC mode
                aes.Padding = PaddingMode.None;         // No padding (CRITICAL for this step)

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    cs.Write(data, 0, data.Length);     // Write the data to the stream
                    cs.FlushFinalBlock();               // Ensure all data is encrypted
                    return ms.ToArray();
                }
            }
        } catch (Exception ex) {
            throw new InvalidOperationException("Encryption failed for Algorithm 2.B, Step (b).", ex);
        }
    }

    /// <summary>
    /// Encrypts data using AES-128-CBC encryption with PKCS#7 padding.
    /// </summary>
    /// <param name="plain">Plaintext data to encrypt</param>
    /// <param name="key">128-bit (16-byte) encryption key</param>
    /// <returns>An EncryptedDataWithIV object containing the encrypted data and generated IV</returns>
    internal static EncryptedDataWithIV EncryptAes128(byte[] plain, byte[] key) {
        if (plain == null || plain.Length == 0) {
            throw new ArgumentException("Plaintext data cannot be empty for encryption.", nameof(plain));
        }
        if (key == null || key.Length != 16) { // 128 bits = 16 bytes
            throw new ArgumentException("Key must be 128 bits (16 bytes) long.", nameof(key));
        }

        // Generate a random 16-byte IV (128 bits)
        byte[] iv = RandomNumberGenerator.GetBytes(16);

        // Perform encryption
        byte[] encryptedData = Encrypt(plain, 128, key, iv);

        // Return the encrypted data and generated IV wrapped in an EncryptedDataWithIV object
        return new EncryptedDataWithIV(encryptedData, iv);
    }

    /// <summary>
    /// Encrypts data using AES-256-CBC encryption with PKCS#7 padding.
    /// </summary>
    /// <param name="plain">Plaintext data to encrypt</param>
    /// <param name="key">256-bit (32-byte) encryption key</param>
    /// <returns>Encrypted ciphertext along with the initialization vector (IV) used for encryption</returns>
    internal static EncryptedDataWithIV EncryptAes256(byte[] plain, byte[] key) {
        if (plain == null || plain.Length == 0) {
            throw new ArgumentException("Plaintext data cannot be empty for encryption.", nameof(plain));
        }
        if (key == null || key.Length != 32) { // 256 bits = 32 bytes
            throw new ArgumentException("Key must be 256 bits (32 bytes) long.", nameof(key));
        }

        // Generate a random 16-byte IV for AES-256
        byte[] iv = RandomNumberGenerator.GetBytes(16);

        // Encrypt the plaintext using AES-256-CBC
        byte[] encryptedData = Encrypt(plain, 256, key, iv);

        // Return the encrypted data and the IV together in an EncryptedDataWithIV object
        return new EncryptedDataWithIV(encryptedData, iv);
    }

    /// <summary>
    /// Encrypts data with AES-CBC and PKCS#7 padding using specified key size.
    /// </summary>
    /// <param name="plain">Plaintext data to encrypt</param>
    /// <param name="keySize">AES key size (128 or 256 bits)</param>
    /// <param name="key">Encryption key (must match keySize length)</param>
    /// <param name="iv">128-bit initialization vector</param>
    /// <returns>Encrypted ciphertext</returns>
    private static byte[] Encrypt(byte[] plain, int keySize, byte[] key, byte[] iv) {
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

    /// <summary>
    /// Decrypts data encrypted with AES-CBC and PKCS#7 padding using specified key size.
    /// </summary>
    /// <param name="ciphertext">Encrypted data to decrypt</param>
    /// <param name="keySize">AES key size (128 or 256 bits)</param>
    /// <param name="key">Decryption key (must match keySize length)</param>
    /// <param name="iv">128-bit initialization vector</param>
    /// <returns>Decrypted plaintext</returns>
    private static byte[] Decrypt(byte[] ciphertext, int keySize, byte[] key, byte[] iv) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = keySize;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                cs.Write(ciphertext, 0, ciphertext.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }
}
}
