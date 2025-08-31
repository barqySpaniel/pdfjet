using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
public class AES {
//    private readonly byte[] key;   // 128-bit AES key
//    private readonly byte[] iv;    // 128-bit IV
//
//    /// <summary>
//    /// Performs the encryption for Step (b) of Algorithm 2.B.
//    /// Encrypts the input data using AES-128-CBC with no padding, using the provided key and IV.
//    /// </summary>
//    /// <param name="data">The data to encrypt (the K1 array).</param>
//    /// <param name="key">The 16-byte AES key.</param>
//    /// <param name="iv">The 16-byte initialization vector.</param>
//    /// <returns>The ciphertext result E.</returns>
//    internal static byte[] EncryptAlgorithmStep2B(byte[] data, byte[] key, byte[] iv) {
//        // Input validation
//        // Algorithm 2.B always uses AES-128-CBC to encrypt K1, regardless of the file key length (AES-128 or AES-256)
//        if (key.Length != 16) throw new ArgumentException(
//            "Key must be 16 bytes for AES-128-CBC (per Algorithm 2.B).", nameof(key));
//        if (iv.Length != 16) throw new ArgumentException("IV must be 16 bytes.", nameof(iv));
//
//        using (Aes aes = Aes.Create()) {
//            // Configure EXACTLY as specified for Algorithm 2.B, Step (b)
//            aes.KeySize = 128;              // Must be AES-128
//            aes.Key = key;
//            aes.IV = iv;
//            aes.Mode = CipherMode.CBC;      // Must be CBC mode
//            aes.Padding = PaddingMode.None; // CRITICAL: No padding
//
//            using (ICryptoTransform encryptor = aes.CreateEncryptor())
//            using (MemoryStream ms = new MemoryStream())
//            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
//                // Encrypt the entire data array without padding
//                cs.Write(data, 0, data.Length);
//                cs.FlushFinalBlock();       // Still necessary to process all data
//                return ms.ToArray();
//            }
//        }
//    }

    /// <summary>
    /// Encrypts data using AES-128-CBC encryption with PKCS#7 padding.
    /// </summary>
    /// <param name="plain">Plaintext data to encrypt</param>
    /// <param name="key">128-bit (16-byte) encryption key</param>
    /// <param name="iv">128-bit (16-byte) initialization vector</param>
    /// <returns>Encrypted ciphertext</returns>
    internal static byte[] EncryptAes128(byte[] plain, byte[] key, byte[] iv) {
        if (key == null || key.Length != 16) { // 128 bits = 16 bytes
            throw new ArgumentException("Key must be 128 bits (16 bytes) long.", nameof(key));
        }
        if (iv == null || iv.Length != 16) {   // 128 bits = 16 bytes
            throw new ArgumentException("IV must be 128 bits (16 bytes) long.", nameof(iv));
        }
        return Encrypt(plain, 128, key, iv);
    }

    /// <summary>
    /// Encrypts data using AES-256-CBC encryption with PKCS#7 padding.
    /// </summary>
    /// <param name="plain">Plaintext data to encrypt</param>
    /// <param name="key">256-bit (32-byte) encryption key</param>
    /// <param name="iv">128-bit (16-byte) initialization vector</param>
    /// <returns>Encrypted ciphertext</returns>
    internal static byte[] Encrypt(byte[] plain, byte[] key, byte[] iv) {
        if (key == null || key.Length != 32) { // 256 bits = 32 bytes
            throw new ArgumentException("Key must be 256 bits (32 bytes) long.", nameof(key));
        }
        if (iv == null || iv.Length != 16) {   // 128 bits = 16 bytes
            throw new ArgumentException("IV must be 128 bits (16 bytes) long.", nameof(iv));
        }
        return Encrypt(plain, 256, key, iv);
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

//    // === Helpers ===
//    private static byte[] HashPassword(byte[] input) {
//        using (SHA256 sha256 = SHA256.Create()) {
//            return sha256.ComputeHash(input);
//        }
//    }
//
//    private static byte[] getSalt() {
//        // 1. User's password (convert to bytes using UTF-8 encoding)
//        string password = "MySecurePassword!123";
//        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
//
//        // 2. Generate a random 16-byte salt (as per PDF 2.0)
//        byte[] salt = RandomNumberGenerator.GetBytes(16);
//        Console.WriteLine($"Generated Salt (Base64): {Convert.ToBase64String(salt)}");
//
//        // 3. Set the high iteration count (PDF 2.0 recommends >= 100,000)
//        int iterations = 100_000;
//
//        // 4. Derive a 32-byte (256-bit) key using PBKDF2 with HMAC-SHA256
//        byte[] aes256Key;
//        using (var derivedBytes = new Rfc2898DeriveBytes(
//                passwordBytes,
//                salt,
//                iterations,
//                HashAlgorithmName.SHA256)) {
//            aes256Key = derivedBytes.GetBytes(32); // 32 bytes = 256 bits
//            Console.WriteLine($"Derived AES-256 Key (Base64): {Convert.ToBase64String(aes256Key)}");
//            Console.WriteLine($"Iterations used: {iterations}");
//        }
//        return aes256Key;
//    }
}
}
