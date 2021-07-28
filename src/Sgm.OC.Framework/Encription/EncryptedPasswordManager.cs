using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace  Sgm.OC.Framework.Encription
{
    public class EncryptedPasswordManager
    {
        private byte[] key;
        private byte[] iv;

        private static readonly byte[] IVPattern = new byte[]
        {
            0x23, 0x01, 0x0a, 0x0e, 0x1d, 0x08, 0x4e, 0x0b, 0x08, 0x06, 0x15, 0x4e, 0x0e, 0x04, 0x07, 0x0d
        };


        protected EncryptedPasswordManager(string key) : this(Encoding.UTF8.GetBytes(key))
        {

        }

        protected EncryptedPasswordManager(byte[] key) : this(key, GenerateIV(key))
        {
            
        }

        protected EncryptedPasswordManager(byte[] key, byte[] iv)
        {
            using (AesManaged aes = new AesManaged())
            {
                this.key = ExtendToBlockSize(key, aes.BlockSize / 8);
                this.iv = ExtendToBlockSize(iv, aes.BlockSize / 8);
            }
        }

        /// <summary>
        /// Crea un manager que utilice la contraseña especificada
        /// </summary>
        /// <param name="password">La contraseña a utilizar</param>
        /// <returns>Una instancia de EncryptedPasswordManager</returns>
        public static EncryptedPasswordManager ForPassword(string password)
        {
            return new EncryptedPasswordManager(password);
        }

        /// <summary>
        /// Genera un IV para la contraseña basándose en ciertos patrones de ésta.
        /// </summary>
        private static byte[] GenerateIV(byte[] pass)
        {
            byte[] IV = new byte[IVPattern.Length];
            for (int i = 0; i < IV.Length; ++i)
            {
                IV[i] = (byte)((pass[IVPattern[i] % pass.Length] ^ 0xd4) & 0xff);
            }
            return IV;
        }

        /// <summary>
        /// Encripta una contraseña en texto plano UTF-8.
        /// </summary>
        public string Encrypt(string password)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                password = ExtendToBlockSize(password, aes.BlockSize / 8);
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(password);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Desencripta una contraseña en texto plano UTF-8.
        /// </summary>
        /// <param name="password">La contraseña a desencriptar en base64</param>
        /// <returns>La contraseña desencriptada</returns>
        public string Decrypt(string password, bool forweb = false)
        {
            string plaintext = null;

            byte[] pBytes = forweb ? Convert.FromBase64String(HttpUtility.UrlDecode(password)) : Convert.FromBase64String(password);
            
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream ms = new MemoryStream(pBytes))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }

            // Para sacar datos de padding
            return plaintext.TrimEnd();
        }



        private string ExtendToBlockSize(string data, int blockSize)
        {
            if (data.Length % blockSize == 0)
                return data;
            var builder = new StringBuilder(data);
            if ((data.Length % blockSize) != 0)
            {
                builder.Append(' ', blockSize - data.Length % blockSize);
            }
            return builder.ToString();
        }

        private static byte[] ExtendToBlockSize(byte[] data, int blockSize)
        {
            if (data.Length % blockSize == 0)
                return data;
            var newData = new byte[data.Length + blockSize - data.Length % blockSize];
            Array.Copy(data, newData, data.Length);
            // Repetir el password hasta rellenar el bloque
            for (int i = data.Length; i < newData.Length; i += data.Length)
            {
                Array.Copy(data, 0, newData, i, Math.Min(data.Length, newData.Length - i));
            }
            return newData;
        }


        private byte[] UrlTokenDecode(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int len = input.Length;
            if (len < 1)
                return new byte[0];

            ///////////////////////////////////////////////////////////////////
            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return null;


            ///////////////////////////////////////////////////////////////////
            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];


            ////////////////////////////////////////////////////////
            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            ////////////////////////////////////////////////////////
            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            // Do the actual conversion
            return Convert.FromBase64CharArray(base64Chars, 0, base64Chars.Length);
        }

    }
}
