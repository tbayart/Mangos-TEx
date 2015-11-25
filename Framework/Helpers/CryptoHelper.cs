using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Framework.Helpers
{
    public static class CryptoHelper
    {
        public static string Encrypt(this ICryptoTransform crypto, SecureString toEncrypt)
        {
            using (crypto)
            using (MemoryStream memStream = new MemoryStream())
            using (CryptoStream crypStream = new CryptoStream(memStream, crypto, CryptoStreamMode.Write))
            {
                IntPtr bstr = Marshal.SecureStringToBSTR(toEncrypt);

                try
                {
                    byte b;
                    for (int index = 0; index < toEncrypt.Length * 2; index = index + 2)
                    {
                        b = Marshal.ReadByte(bstr, index);
                        crypStream.WriteByte(b);
                    }
                    b = 0;

                    crypStream.FlushFinalBlock();
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }

                return Convert.ToBase64String(memStream.ToArray());
            }
        }

        public static SecureString Decrypt(this ICryptoTransform crypto, string toDecrypt)
        {
            using (crypto)
            using (MemoryStream memStream = new MemoryStream())
            using (CryptoStream crypStream = new CryptoStream(memStream, crypto, CryptoStreamMode.Write))
            {
                var buffer = Convert.FromBase64String(toDecrypt);
                crypStream.Write(buffer, 0, buffer.Length);
                crypStream.FlushFinalBlock();
                SecureString result = new SecureString();

                memStream.Seek(0, SeekOrigin.Begin);
                Char b;
                for (int index = 0; index < memStream.Length; ++index)
                {
                    b = Convert.ToChar(memStream.ReadByte());
                    result.AppendChar(b);
                }
                b = Char.MinValue;

                return result;
            }
        }

        public static string DecryptClear(this ICryptoTransform crypto, string toDecrypt)
        {
            using (crypto)
            using (MemoryStream memStream = new MemoryStream())
            using (CryptoStream crypStream = new CryptoStream(memStream, crypto, CryptoStreamMode.Write))
            {
                var buffer = Convert.FromBase64String(toDecrypt);
                crypStream.Write(buffer, 0, buffer.Length);
                crypStream.FlushFinalBlock();
                memStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(memStream))
                    return sr.ReadToEnd();
            }
        }
    }
}
