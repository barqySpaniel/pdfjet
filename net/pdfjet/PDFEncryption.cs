using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDFjet.NET {
    public class PDFEncryption {
        private byte[] fileKey;   // 32-byte file encryption key
        private byte[] iv;        // 16-byte IV
        private int objNumber;

        public PDFEncryption(PDF pdf, string userPassword, string ownerPassword) {
            // 1. Generate random 32-byte file encryption key
            fileKey = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(fileKey);
            }

            // 2. Generate random IV (AES block size = 128-bit)
            iv = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(iv);
            }

            // 3. Compute password hashes
            byte[] ownerHash = HashPassword(Encoding.UTF8.GetBytes(ownerPassword));
            byte[] userHash  = HashPassword(Encoding.UTF8.GetBytes(userPassword));

            // 4. Compute /OE and /UE
            byte[] OE = AESEncrypt(fileKey, ownerHash, iv);
            byte[] UE = AESEncrypt(fileKey, userHash, iv);

            // 5. Create /Perms (16-byte, placeholder zeros for now)
            byte[] perms = new byte[16];

            // 6. Write encryption dictionary
            pdf.NewObj();
            pdf.Append(Token.BeginDictionary);

            pdf.Append("/Filter /Standard\n");
            pdf.Append("/V 5\n");
            pdf.Append("/R 5\n");
            pdf.Append("/Length 256\n");
            pdf.Append("/CF << /StdCF << /CFM /AESV3 /AuthEvent /DocOpen /Length 32 >> >>\n");
            pdf.Append("/StmF /StdCF\n");
            pdf.Append("/StrF /StdCF\n");

            pdf.Append("/O <" + ToHex(ownerHash) + ">\n");
            pdf.Append("/U <" + ToHex(userHash) + ">\n");
            pdf.Append("/OE <" + ToHex(OE) + ">\n");
            pdf.Append("/UE <" + ToHex(UE) + ">\n");
            pdf.Append("/Perms <" + ToHex(perms) + ">\n");

            pdf.Append(Token.EndDictionary);
            pdf.EndObj();

            objNumber = pdf.GetObjNumber();
        }

        private byte[] HashPassword(byte[] input) {
            using (SHA256 sha256 = SHA256.Create()) {
                return sha256.ComputeHash(input);
            }
        }

        private byte[] AESEncrypt(byte[] data, byte[] key, byte[] iv) {
            using (Aes aes = Aes.Create()) {
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public int GetObjNumber() { return objNumber; }

        private string ToHex(byte[] bytes) {
            char[] hex = new char[bytes.Length * 2];
            const string HEX_CHARS = "0123456789ABCDEF";
            for (int i = 0; i < bytes.Length; i++) {
                hex[i * 2] = HEX_CHARS[bytes[i] >> 4];
                hex[i * 2 + 1] = HEX_CHARS[bytes[i] & 0x0F];
            }
            return new string(hex);
        }

        public byte[] Encrypt(byte[] plain) { return AESEncrypt(plain, fileKey, iv); }
    }
}
