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
    public EncryptedDataWithIV(byte[] encryptedData, byte[] iv) {
        this.encryptedData = encryptedData;
        this.iv = iv;
    }
}

public class AES {
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
    internal static byte[] EncryptK1(byte[] K1, byte[] key, byte[] iv) {
        if (K1 == null || K1.Length == 0) {
            throw new ArgumentException(
                "K1 cannot be empty for encryption.", nameof(K1));
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
                aes.KeySize = 128;                  // Use AES-128 (128-bit key)
                aes.Key = key;                      // Set the encryption key
                aes.IV = iv;                        // Set the provided initialization vector (IV)
                aes.Mode = CipherMode.CBC;          // Use CBC mode
                aes.Padding = PaddingMode.None;     // No padding (CRITICAL for this step)

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    cs.Write(K1, 0, K1.Length);     // Write the data to the stream
                    cs.FlushFinalBlock();           // Ensure all data is encrypted
                    return ms.ToArray();
                }
            }
        } catch (Exception ex) {
            throw new InvalidOperationException("Encryption failed for Algorithm 2.B, Step (b).", ex);
        }
    }

    /// <summary>
    /// Encrypts data using AES-256-CBC encryption with PKCS #5 padding.
    /// </summary>
    /// <param name="data">The data to encrypt</param>
    /// <param name="key">256-bit (32-byte) encryption key</param>
    /// <returns>Encrypted ciphertext along with the initialization vector (IV) used for encryption</returns>
    internal static EncryptedDataWithIV EncryptAes256(byte[] data, byte[] key) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException("The data cannot be empty for encryption.", nameof(data));
        }
        if (key == null || key.Length != 32) { // 256 bits = 32 bytes
            throw new ArgumentException("Key must be 256 bits (32 bytes) long.", nameof(key));
        }

        // Generate a random 16-byte IV for AES-256
        byte[] iv = RandomNumberGenerator.GetBytes(16);

        // Encrypt the data using AES-256-CBC
        byte[] encryptedData = Encrypt(data, key, iv);

        // Return the encrypted data and the IV together in an EncryptedDataWithIV object
        return new EncryptedDataWithIV(encryptedData, iv);
    }

    /// <summary>
    /// Encrypts data with AES-256-CBC and PKCS #5 padding using specified key size.
    /// </summary>
    /// <param name="data">The data to encrypt</param>
    /// <param name="key">256-bit Encryption Key</param>
    /// <param name="iv">128-bit initialization vector</param>
    /// <returns>Encrypted ciphertext</returns>
    private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;    // PKCS #5 and PKCS #7 are considered equivalent,
                                                // especially in the context of AES encryption.
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Encrypts the File Encryption Key (FEK) using AES-256-CBC with a zero IV and no padding.
    /// </summary>
    /// <param name="fileEncryptionKey">The 32-byte File Encryption Key.</param>
    /// <param name="key">The 32-byte hash used as the encryption key.</param>
    /// <returns>The resulting 32-byte UE (User Encryption) key.</returns>
    public static byte[] EncryptKeyWithZeroIV(byte[] fileEncryptionKey, byte[] key) {
        // Validate inputs
        if (fileEncryptionKey == null || fileEncryptionKey.Length != 32) {
            throw new ArgumentException(
                "File Encryption Key must be 32 bytes long.", nameof(fileEncryptionKey));
        }
        if (key == null || key.Length != 32) {
            throw new ArgumentException(
                "The encryption key must be 32 bytes long.", nameof(key));
        }

        // Create the AES object with the specific parameters
        using (Aes aes = Aes.Create()) {
            aes.Key = key;
            aes.KeySize = 256;              // Use AES-256 (32 bytes key)
            aes.IV = new byte[16];          // new byte[16] initializes all elements to 0.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None; // No padding because input is exact multiple of block size.

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aes.CreateEncryptor();

            // Encrypt the FEK. The output will be the same length as the input (32 bytes).
            return encryptor.TransformFinalBlock(fileEncryptionKey, 0, fileEncryptionKey.Length);
        }
    }

    /// <summary>
    /// Decrypts data encrypted with AES-256-CBC and PKCS #5 padding.
    /// </summary>
    /// <param name="ciphertext">Encrypted data to decrypt</param>
    /// <param name="key">256-bit Decryption Key</param>
    /// <param name="iv">128-bit Initialization Vector</param>
    /// <returns>Decrypted data</returns>
    private static byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;    // PKCS #5 and PKCS #7 are considered equivalent,
                                                // especially in the context of AES encryption.
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
