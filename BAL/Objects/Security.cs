using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class Security
    {
        static string secretKey = ConfigurationManager.AppSettings["encKey"].ToString();


        public static string Decrypt(string encryptedText)
        {
            //Secret Key.
            //   string secretKey = "mysupersecretkey";

            //Secret Bytes.
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);

            //Encrypted Bytes.
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            //Decrypt with AES Alogorithm using Secret Key.
            using (Aes aes = Aes.Create())
            {
                aes.Key = secretBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] decryptedBytes = null;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                }

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
        public static string Encrypt(string plainText)
        {
            //Secret Key.
            //   string secretKey = "mysupersecretkey";

            //Secret Bytes.
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);

            //Plain Text Bytes.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //Encrypt with AES Alogorithm using Secret Key.
            using (Aes aes = Aes.Create())
            {
                aes.Key = secretBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] encryptedBytes = null;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                }

                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }
}
