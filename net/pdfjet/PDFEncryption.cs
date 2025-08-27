using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
        // === Derive AES-128 key from user+owner password and docID ===
        using (SHA256 sha256 = SHA256.Create()) {
            byte[] fullHash = sha256.ComputeHash(Combine(
                Encoding.UTF8.GetBytes(userPassword + ownerPassword),
                Encoding.UTF8.GetBytes(pdf.uuid)   // docID
            ));
            this.key = new byte[16];          // AES-128
            Array.Copy(fullHash, this.key, 16);
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
        pdf.Append("/V 4\n");                  // Algorithm version: AES
        pdf.Append("/R 4\n");                  // Revision 4 (AES-128)
        pdf.Append("/Length 128\n");           // Key length in bits
        pdf.Append("/P -3904\n");              // Permissions (example value)
        pdf.Append("/CF << /StdCF << /CFM /AESV2 /AuthEvent /DocOpen /Length 16 >> >>\n");
        pdf.Append("/StmF /StdCF\n");
        pdf.Append("/StrF /StdCF\n");

        // === User key (U) ===
        pdf.Append("/U <");
        pdf.Append(ToHex(HashPassword(this.key)));
        pdf.Append(">\n");

        // === Owner key (O) ===
        pdf.Append("/O <");
        pdf.Append(ToHex(HashPassword(Encoding.UTF8.GetBytes(ownerPassword))));
        pdf.Append(">\n");

        pdf.Append(">>\n");
        pdf.EndObj();

        objNumber = pdf.GetObjNumber();
    }

    /// <summary>
    /// Encrypts data with AES-128-CBC and PKCS#7 padding.
    /// </summary>
    public byte[] Encrypt(byte[] plain) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = 128;
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

    public int GetObjNumber() {
        return objNumber;
    }

    // === Helpers ===
    private byte[] HashPassword(byte[] input) {
        using (SHA256 sha256 = SHA256.Create()) {
            return sha256.ComputeHash(input);
        }
    }

    private byte[] Combine(byte[] a, byte[] b) {
        byte[] combined = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, combined, 0, a.Length);
        Buffer.BlockCopy(b, 0, combined, a.Length, b.Length);
        return combined;
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
