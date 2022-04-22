using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.Tools.Crypto
{
    internal class AESCrypto : IDisposable
    {
        private byte[] iv;
        private bool disposedValue;

        internal AESCrypto(string iv = "")
        {
            this.iv = GetStringBytes(iv);
        }

        internal AESCrypto(byte[] iv = null)
        {
            if (iv == null)
            {
                this.iv = new byte[32];
            }
            else
            {
                this.iv = GetBytes(iv);
            }
        }

        private byte[] GetBytes(byte[] data, int length = 32)
        {
            byte[] result = new byte[length];
            if (data == null)
            {
                return null;
            }
            if (data.Length > length)
            {
                Array.Copy(data, 0, result, 0, length);
            }
            else
            {
                Array.Copy(data, 0, result, 0, data.Length);
            }
            return result;
        }

        private byte[] GetStringBytes(string data, int length = 32)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            return GetBytes(byteData, length);
        }

        internal byte[] Encrypt(byte[] data, string key)
        {
            byte[] byteKey = GetStringBytes(key);
            byte[] encrypt = null;
            Aes aes = Aes.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(byteKey, this.iv), CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                    cStream.FlushFinalBlock();
                    encrypt = mStream.ToArray();
                }
            }
            aes.Clear();
            return encrypt;
        }

        internal byte[] Decrypt(byte[] data, string key)
        {
            byte[] byteKey = GetStringBytes(key);
            byte[] decrypt = null;
            Aes aes = Aes.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(byteKey, this.iv), CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                    cStream.FlushFinalBlock();
                    decrypt = mStream.ToArray();
                }
            }
            aes.Clear();
            return decrypt;
        }


        // 实现 IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~AESCrypto()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
