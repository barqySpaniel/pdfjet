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
    /// Encrypts <paramref name="K1"/> with AES‑128‑CBC, **no padding**.
    /// </summary>
    /// <param name="K1">Plain‑text to encrypt (must be a multiple of 16 bytes).</param>
    /// <param name="key">Exactly 16 bytes – the AES‑128 key.</param>
    /// <param name="iv">Exactly 16 bytes – the initialization vector.</param>
    /// <returns>The ciphertext.</returns>
    /// <exception cref="ArgumentException">If any argument is null or has an invalid length.</exception>
    internal static byte[] EncryptK1(byte[] K1, byte[] key, byte[] iv) {
        // ---------- basic argument validation ----------
        if (K1 == null || K1.Length == 0)
            throw new ArgumentException("K1 cannot be null or empty.", nameof(K1));

        if (key == null || key.Length != 16)
            throw new ArgumentException("Key must be exactly 16 bytes (AES‑128).", nameof(key));

        if (iv == null || iv.Length != 16)
            throw new ArgumentException("IV must be exactly 16 bytes.", nameof(iv));

        // ---------- AES‑128‑CBC, no padding ----------
        using var aes = Aes.Create();       // defaults: CBC, PKCS7, 256‑bit key
        aes.Key = key;                      // automatically sets KeySize = 128
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;     // critical for Algorithm 2.B

        // ---------- encrypt in one shot ----------
        // TransformFinalBlock allocates the output array for us.
        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(K1, 0, K1.Length);
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
    /// Encrypts <paramref name="data"/> with AES‑256‑CBC and PKCS#7 padding.
    /// </summary>
    /// <param name="data">Plain‑text to encrypt.</param>
    /// <param name="key">
    /// 32‑byte (256‑bit) encryption key. The method will throw exception if the length is not 32.
    /// </param>
    /// <param name="iv">
    /// 16‑byte (128‑bit) initialization vector. The method will throw if the length is not 16.
    /// </param>
    /// <returns>Cipher‑text.</returns>
    /// <exception cref="ArgumentException">Invalid key or IV length.</exception>
    private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv) {
        // ----- basic argument validation -------------------------------------------------
        if (key == null || key.Length != 32)
            throw new ArgumentException("Key must be exactly 32 bytes (AES‑256).", nameof(key));

        if (iv == null || iv.Length != 16)
            throw new ArgumentException("IV must be exactly 16 bytes.", nameof(iv));

        // ----- configure AES -------------------------------------------------------------
        using var aes = Aes.Create();       // defaults: CBC, PKCS7, 256‑bit key
        aes.Key = key;                      // automatically sets KeySize = 256
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;    // PKCS#5 ≡ PKCS#7 for AES block size

        // ----- encrypt in one shot (no streams needed) ---------------------------------
        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// Encrypts a 32‑byte File Encryption Key (FEK) with AES‑256‑CBC,
    /// using a zero IV and no padding.
    /// </summary>
    /// <param name="fileEncryptionKey">
    /// 32‑byte FEK to encrypt.
    /// </param>
    /// <param name="key">
    /// 32‑byte key (hash) used for AES‑256 encryption.
    /// </param>
    /// <returns>The encrypted 32‑byte File Encryption Key.</returns>
    public static byte[] EncryptKeyWithZeroIV(byte[] fileEncryptionKey, byte[] key) {
        // Validate inputs (must be exactly 32 bytes)
        if (fileEncryptionKey == null || fileEncryptionKey.Length != 32)
            throw new ArgumentException("File Encryption Key must be 32 bytes long.", nameof(fileEncryptionKey));
        if (key == null || key.Length != 32)
            throw new ArgumentException("The encryption key must be 32 bytes long.", nameof(key));

        // Set up AES‑256 with a zero IV and no padding
        using var aes = Aes.Create();
        aes.Key = key;                 // 256‑bit key
        aes.IV = new byte[16];         // all‑zero IV
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;

        // Perform the encryption in one block
        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(fileEncryptionKey, 0, fileEncryptionKey.Length);
    }

    /// <summary>
    /// Decrypts AES‑256‑CBC data that was padded with PKCS#5/PKCS#7.
    /// </summary>
    /// <param name="ciphertext">The encrypted byte array.</param>
    /// <param name="key">
    /// 32‑byte (256‑bit) decryption key. Must match the key used for encryption.
    /// </param>
    /// <param name="iv">
    /// 16‑byte (128‑bit) initialization vector. Must match the IV used for encryption.
    /// </param>
    /// <returns>The plaintext bytes.</returns>
    private static byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv) {
        // Configure AES‑256‑CBC with PKCS#7 padding (PKCS#5 is a subset of PKCS#7)
        using var aes = Aes.Create();
        aes.Key = key;                     // 256‑bit key
        aes.IV = iv;                       // 128‑bit IV
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;   // PKCS#5 ≡ PKCS#7 for block sizes ≤ 8 bytes

        // Stream‑based decryption: write ciphertext into a CryptoStream,
        // which writes the decrypted bytes into a MemoryStream.
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(ciphertext, 0, ciphertext.Length);
        cs.FlushFinalBlock();              // finalize padding removal

        return ms.ToArray();               // retrieve plaintext
    }
}
}   // End of namespace PDFjet.NET
