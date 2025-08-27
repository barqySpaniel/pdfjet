using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDFjet.NET {
public class PDFEncryption {
    private const int PAD_LENGTH = 32;
    private static readonly byte[] PASSWORD_PADDING = {
        0x28,0xBF,0x4E,0x5E,0x4E,0x75,0x8A,0x41,
        0x64,0x00,0x4E,0x56,0xFF,0xFA,0x01,0x08,
        0x2E,0x2E,0x00,0xB6,0xD0,0x68,0x3E,0x80,
        0x2F,0x0C,0xA9,0xFE,0x64,0x53,0x69,0x7A
    };
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
        byte[] userPad = PadPassword(userPassword);
        byte[] ownerPad = PadPassword(ownerPassword);

        // === Derive AES-128 key from user+owner password and uuid ===
        using (SHA256 sha256 = SHA256.Create()) {
            byte[] fullHash = sha256.ComputeHash(Combine(
                Combine(PadPassword(userPassword), PadPassword(ownerPassword)),
                Encoding.UTF8.GetBytes(pdf.uuid)
            ));
            this.key = new byte[16];    // AES-128
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

    private void Algorithm2B(byte[] input) {
        // Take the SHA-256 hash of the original input to the algorithm and name the resulting 32 bytes, K.
        byte[] K = HashPassword(input);
        // Perform the following steps (a)-(d) 64 times:
        for (int i = 0; i < 64; i++) {
            // a) Make a new string, K1, consisting of 64 repetitions of the sequence:
            //    input password, K, the 48-byte user key.
            //    The 48 byte user key is only used when checking the owner password or creating the owner key.
            //    If checking the user password or creating the user key,
            //    K1 is the concatenation of the input password and K.

            // b) Encrypt K1 with the AES-128 (CBC, no padding) algorithm,
            //    using the first 16 bytes of K as the key and the second
            //    16 bytes of K as the initialization vector.
            //    The result of this encryption is E.

            // c) Taking the first 16 bytes of E as an unsigned big-endian integer,
            //    compute the remainder, modulo 3.
            //    If the result is 0, the next hash used is SHA-256,
            //    if the result is 1, the next hash used is SHA-384,
            //    if the result is 2, the next hash used is SHA-512.

            // d) Using the hash algorithm determined in step c, take the hash of E.
            //    The result is a new value of K, which will be 32, 48, or 64 bytes in length.
        }

        // Repeat the process (a-d) with this new value for K.
        // Following 64 rounds (round number 0 to round number 63),
        // do the following, starting with round number 64:

        // e) Look at the very last byte of E.
        //    If the value of that byte (taken as an unsigned integer)
        //    is greater than the round number - 32, repeat steps (a-d) again.

        // f) Repeat from steps (a-e) until the value of the last byte is ≤ (round number) - 32.

        // NOTE 3
        // Tests indicate that the total number of rounds will most likely be between 65 and 80.
        // So we can print this number to verify we are in this range!
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

    private byte[] PadPassword(string password) {
        byte[] pwd = Encoding.UTF8.GetBytes(password ?? "");
        byte[] padded = new byte[PAD_LENGTH];
        int len = Math.Min(pwd.Length, PAD_LENGTH);
        if (len > 0) Array.Copy(pwd, 0, padded, 0, len);
        if (len < PAD_LENGTH) Array.Copy(PASSWORD_PADDING, 0, padded, len, PAD_LENGTH - len);
        return padded;
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
