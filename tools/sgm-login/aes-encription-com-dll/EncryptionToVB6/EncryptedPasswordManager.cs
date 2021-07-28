using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Net.Http.Headers;
using System.Web.Management;
using System.Reflection;

namespace EncryptionToVB6
{
    /// <summary>
    /// Interfaz para exposicion a VB6
    /// </summary>
    [Guid("C884FEDF-4B8D-4C45-BB8A-DC33DBFBDC8A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    public interface IEncryptedPasswordManager
    {
        [DispId(1)]
        string Encryptation(string user, int sucId);
    }

    [Guid("B1B5A13C-132E-478A-9F13-8F0B5F8DFC7F")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ProgId("SGMOC.Encriptacion")]
    public class EncryptedPasswordManager : IEncryptedPasswordManager
    {
        /// <summary>
        /// Construuctor para exposicion a VB6
        /// </summary>
        public EncryptedPasswordManager()
        {

        }
        private byte[] key;
        private byte[] iv;
        private int SucursalId;

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
        /// Metodo que retonar un texto encriptado para login desde Modulo SGM Destokp hacia SGM-OC Web
        /// </summary>
        /// <param name="textToChyper">Texto a cifrar</param>
        /// <param name="key">Key del cifrado</param>
        /// <returns><see cref="String"/>String encriptado</returns>
        public string Encryptation(string user, int sucId)
        {
            string key = string.Empty;
            this.SucursalId = sucId;
#if DEBUG
            key = Properties.Resources.TestingKey;
#else
            key = Properties.Resources.ProducctionKey;
#endif

            string encryptedStr = ForPassword(key).Encrypt(AssemblyJson(user));

            return HttpUtility.UrlEncode(encryptedStr);
        }

        /// <summary>
        /// Metodo que arma y retorna el Json a encriptar
        /// </summary>
        /// <param name="user"></param>
        /// <returns><see cref="String"/></returns>
        private string AssemblyJson(string user)
        {
            int expMinutes = 5; //Pueden configurarlo en el appSetting.json
            var jsonObj = new
            {
                login = user,
                sucursalId = this.SucursalId,
                date = DateTime.Now.ToBinary(),
                expirationDate = DateTime.Now.AddMinutes(expMinutes).ToBinary(),
                expiration = expMinutes
            };
            return JsonConvert.SerializeObject(jsonObj).ToString();
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

            //return HttpServerUtility.UrlTokenEncode(encrypted);
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

            //byte[] pBytes = Convert.FromBase64String(password); //HttpServerUtility.UrlTokenDecode(password);
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

    }
}
