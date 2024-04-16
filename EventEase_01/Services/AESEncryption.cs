using System;
using System.IO;
using System.Security.Cryptography;

namespace EventEase_01.Services
{
    public class AESEncryption
    {
        private readonly string _passwordKey;

        public AESEncryption(string passwordKey)
        {
            _passwordKey = passwordKey;
        }

        //public string Encrypt(string password, out string iv)
        //{
        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Padding = PaddingMode.Zeros;
        //        aes.Key = Convert.FromBase64String(_passwordKey);
        //        aes.GenerateIV();
        //        iv = Convert.ToBase64String(aes.IV);
        //        ICryptoTransform encryptor = aes.CreateEncryptor();
        //        byte[] encryptedData;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter sw = new StreamWriter(cs))
        //                {
        //                    sw.Write(password);
        //                }
        //                encryptedData = ms.ToArray();
        //            }
        //        }
        //        return Convert.ToBase64String(encryptedData);
        //    }
        //}
        public string Encrypt(string password, out string iv, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(key);
                aes.GenerateIV();
                iv = Convert.ToBase64String(aes.IV);
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] encryptedData;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(password);
                        }
                        encryptedData = ms.ToArray();
                    }
                }
                return Convert.ToBase64String(encryptedData);
            }
        }
        //public string AuthEncrypt(string password, string iv)
        //{
        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Padding = PaddingMode.Zeros;
        //        aes.Key = Convert.FromBase64String(_passwordKey);
        //        aes.IV = Convert.FromBase64String(iv);
        //        ICryptoTransform encryptor = aes.CreateEncryptor();
        //        byte[] encryptedData;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter sw = new StreamWriter(cs))
        //                {
        //                    sw.Write(password);
        //                }
        //                encryptedData = ms.ToArray();
        //            }
        //        }
        //        return Convert.ToBase64String(encryptedData);
        //    }
        //}
        public string AuthEncrypt(string password, in string iv, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] encryptedData;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(password);
                        }
                        encryptedData = ms.ToArray();
                    }
                }
                return Convert.ToBase64String(encryptedData);
            }
        }
    }
}
