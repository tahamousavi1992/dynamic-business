using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.Domain
{
    public static class StringCipher
    {
        public static string EncryptFormValues(string plainText, string apiSessionId, bool isEncrypted)
        {
            return isEncrypted ? StringCipher.Encrypt(plainText, apiSessionId + "_adsk256") : plainText;
        }

        public static string DecryptFormValues(string cipherText, string apiSessionId, bool isEncrypted)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) return cipherText;
            if (!isEncrypted) return
                    (cipherText.Length > 60 &&
                    !string.IsNullOrWhiteSpace(StringCipher.Decrypt(cipherText, apiSessionId + "_adsk256")) ?
                    StringCipher.Decrypt(cipherText, apiSessionId + "_adsk256") : cipherText);
            string result = StringCipher.Decrypt(cipherText, apiSessionId + "_adsk256");
            return string.IsNullOrWhiteSpace(result) ? cipherText : result;
        }

        public static string Encrypt(string plainText, string encryptionKey)
        {
            //string EncryptionKey = "tahaahat";

            if (plainText == null) return string.Empty;

            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(plainText);

            using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
            {
                System.Security.Cryptography.Rfc2898DeriveBytes pdb = new System.Security.Cryptography.Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, encryptor.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainText = HttpServerUtility.UrlTokenEncode(ms.ToArray());
                }
            }
            return plainText;
        }

        public static string Decrypt(string cipherText, string encryptionKey)
        {
            try
            {
                if (cipherText == null) return string.Empty;

                byte[] cipherBytes = HttpServerUtility.UrlTokenDecode(cipherText.ToString());

                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    System.Security.Cryptography.Rfc2898DeriveBytes pdb = new System.Security.Cryptography.Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, encryptor.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = System.Text.Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch
            {
                cipherText = string.Empty;
            }
            return cipherText.ToString();
        }

    }
}