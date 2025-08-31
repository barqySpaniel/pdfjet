using System;
using System.Security.Cryptography;

/// <summary>
/// Simple holder for the key‑derivation output required by PDF 2.0.
/// Uses public fields (no getters/setters) as you requested.
/// </summary>
public sealed class PdfKeyMaterial
{
    public byte[] Key;        // 32‑byte AES‑256 key
    public byte[] Salt;       // 16‑byte random salt
    public int    Iterations; // PBKDF2 iteration count
}

public static class PdfEncryptionHelper {
    /// <summary>
    /// Derives the encryption material for a PDF 2.0 document.
    /// The caller supplies only the password (as a byte array);
    /// the method creates its own salt and fills a PdfKeyMaterial instance.
    /// </summary>
    /// <param name="passwordBytes">Password already encoded (e.g., UTF‑8).</param>
    /// <param name="keySizeBytes">Length of the derived key (default 32 bytes for AES‑256).</param>
    /// <returns>An instance of PdfKeyMaterial containing key, salt, and iteration count.</returns>
    public static PdfKeyMaterial DeriveKeyMaterial(
        byte[] passwordBytes,
        int keySizeBytes = 32)   // 32 bytes = 256 bits (AES‑256)
    {
        if (passwordBytes == null || passwordBytes.Length != 16) {  // TODO: or 32 ?
            throw new ArgumentNullException(nameof(passwordBytes));
        }

        // PDF 2.0 recommends at least 100000 iterations.
        const int iterations = 100000;

        // ---------------------------------------------------------
        // 1) Generate a fresh 16‑byte (128‑bit) salt.
        // ---------------------------------------------------------
        byte[] salt = RandomNumberGenerator.GetBytes(16); // secure RNG

        // ---------------------------------------------------------
        // 2) Run PBKDF2‑HMAC‑SHA‑256.
        // ---------------------------------------------------------
        using var kdf = new Rfc2898DeriveBytes(
            passwordBytes,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        byte[] key = kdf.GetBytes(keySizeBytes);

        // ---------------------------------------------------------
        // 3) (Optional) wipe the password buffer now that we’re done.
        // ---------------------------------------------------------
        // Array.Clear(passwordBytes, 0, passwordBytes.Length);

        // Fill the simple field‑based container.
        var result = new PdfKeyMaterial();
        result.Key        = key;
        result.Salt       = salt;
        result.Iterations = iterations;

        return result;
    }
}
