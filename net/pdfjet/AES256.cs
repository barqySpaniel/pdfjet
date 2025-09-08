using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace PDFjet.NET {
public class AES256 {
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
    internal static byte[] EncryptWithZeroIV(byte[] fileEncryptionKey, byte[] key) {
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

    internal static byte[] EncryptAes256(byte[] data, byte[] key) {
        // Generate a random 16-byte IV for AES-256
        byte[] iv = RandomNumberGenerator.GetBytes(16);

        // Configure AES‑256‑CBC with PKCS#7 padding (PKCS#5 is a subset of PKCS#7)
        using var aes = Aes.Create();
        aes.Key = key;                  // 32 bytes key (AES-256)
        aes.IV = iv;                    // 16 bytes IV
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream();
        ms.Write(iv, 0, 16);

        using var encryptor = aes.CreateEncryptor();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }
}
}   // End of namespace PDFjet.NET
