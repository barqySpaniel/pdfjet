using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDFjet.NET {
public class PDFEncryption {
    private byte[] key;       // 256-bit AES key
    private byte[] iv;        // 128-bit IV
    private int objNumber;

    /// <summary>
    /// Creates a new PDFEncryption that embeds an AES-256/SHA-256
    /// based encryption dictionary into the PDF.
    /// </summary>
    /// <param name="pdf">The parent PDF document.</param>
    /// <param name="userPassword">The user password string.</param>
    /// <param name="ownerPassword">The owner password string.</param>
    public PDFEncryption(PDF pdf, string userPassword, string ownerPassword) {
        // Derive AES-256 key using SHA-256
        using (SHA256 sha256 = SHA256.Create()) {
            this.key = sha256.ComputeHash(Encoding.UTF8.GetBytes(userPassword + ownerPassword));
        }

        // Generate random IV (AES block size is 128-bit)
        this.iv = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(this.iv);
        }

        WriteEncryptionDictionary(pdf, ownerPassword, -3904);
    }

    private void WriteEncryptionDictionary(PDF pdf, String ownerPassword, int permissions) {
        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);

        pdf.Append("/Filter /Standard\n");
        pdf.Append("/V 5\n");                  // Algorithm version
        pdf.Append("/R 5\n");                  // Revision (AES-256, SHA-256)
        pdf.Append("/Length 256\n");           // Key length in bits

        pdf.Append("/P ");
        pdf.Append(permissions);
        pdf.Append("\n");

        pdf.Append("/CF << /StdCF << /CFM /AESV3 /AuthEvent /DocOpen /Length 32 >> >>\n");
        pdf.Append("/StmF /StdCF\n");
        pdf.Append("/StrF /StdCF\n");

        // User password hash
        pdf.Append("/U <");
        pdf.Append(ToHex(HashPassword(key)));
        pdf.Append(">\n");

        // Owner password hash
        pdf.Append("/O <");
        pdf.Append(ToHex(HashPassword(Encoding.UTF8.GetBytes(ownerPassword))));
        pdf.Append(">\n");

        pdf.Append(Token.EndDictionary);
        pdf.EndObj();

        objNumber = pdf.GetObjNumber();
    }

    private byte[] HashPassword(byte[] input) {
        using (SHA256 sha256 = SHA256.Create()) {
            return sha256.ComputeHash(input);
        }
    }

    /// <summary>
    /// Encrypts a buffer using AES-256-CBC with the derived key/IV.
    /// </summary>
    public byte[] Encrypt(byte[] plain) {
        using (Aes aes = Aes.Create()) {
            aes.KeySize = 256;
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
