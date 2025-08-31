using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
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
    /// Derives an encryption key from a password using the PBKDF2 key derivation function (KDF).
    /// This method is used to securely generate an AES key from a user password,
    /// which can then be used for AES encryption (AES-256 in this case).
    /// </summary>
    /// <param name="password">The password input provided by the user. It is used as the basis for the key derivation process.</param>
    /// <param name="salt">A unique, random salt used to ensure that the same password results in different derived keys each time. This should be securely generated and stored.</param>
    /// <param name="keySize">The size of the derived key in bytes. Default is 32 bytes, which is required for AES-256 encryption (256-bit key).</param>
    /// <param name="iterations">The number of iterations to run the PBKDF2 function. The higher the number, the more secure the derived key will be, but it will also take longer to compute. The minimum recommended value is 100,000 iterations, but you can increase this for stronger security.</param>
    /// <returns>A byte array containing the derived encryption key. The size of the array is determined by the specified keySize (32 bytes for AES-256).</returns>
    /// <exception cref="ArgumentNullException">Thrown if the password or salt is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the salt length is less than the recommended minimum size.</exception>
    internal static byte[] DeriveKeyFromPassword(
            string password,
            byte[] salt,
            int keySize = 32,           // 32 bytes for AES-256
            int iterations = 100000) {  // Stronger iterations count required by the PDF specification
        // Ensure password and salt are provided (to prevent errors)
        if (password == null) throw new ArgumentNullException(nameof(password), "Password cannot be null.");
        if (salt == null) throw new ArgumentNullException(nameof(salt), "Salt cannot be null.");

        // Ensure the salt length is at least 8 bytes for better security
        if (salt.Length < 8) {
            throw new ArgumentException("Salt must be at least 8 bytes long.", nameof(salt));
        }

        // Create a PBKDF2 key derivation function with the specified password,
        // salt, iterations, and SHA-256 hash algorithm
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)) {
            // Generate the derived key of the requested size (default is 32 bytes for AES-256)
            return pbkdf2.GetBytes(keySize);
        }
    }

    /// <summary>
    /// Performs the encryption for Step (b) of Algorithm 2.B.
    /// Encrypts the input data using AES-128-CBC with no padding, using a randomly generated IV.
    /// </summary>
    /// <param name="plain">The data to encrypt (the K1 array).</param>
    /// <param name="key">The 16-byte AES key.</param>
    /// <returns>The encrypted data and the generated IV used for encryption.</returns>
    internal static EncryptedDataWithIV EncryptAlgorithmStep2B(byte[] plain, byte[] key) {
        // Validate the input parameters
        if (plain == null || plain.Length == 0) {
            throw new ArgumentException("Plaintext data cannot be empty for encryption.", nameof(plain));
        }
        if (key == null || key.Length != 16) {
            throw new ArgumentException("Key must be 16 bytes for AES-128-CBC (per Algorithm 2.B).", nameof(key));
        }

        try {
            // Generate a random 16-byte IV
            byte[] iv = RandomNumberGenerator.GetBytes(16);

            using (Aes aes = Aes.Create()) {
                // Configure EXACTLY as specified for Algorithm 2.B, Step (b)
                aes.KeySize = 128;              // AES-128
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;      // CBC mode
                aes.Padding = PaddingMode.None; // No padding (CRITICAL)

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    cs.Write(plain, 0, plain.Length);
                    cs.FlushFinalBlock();       // Ensures all data is processed

                    // Return the encrypted data and the IV as EncryptedDataWithIV
                    return new EncryptedDataWithIV(ms.ToArray(), iv);
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
}
}
