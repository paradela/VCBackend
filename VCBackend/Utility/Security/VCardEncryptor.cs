using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace VCBackend.Utility.Security
{
    public class VCardEncryptor
    {
        private readonly byte[] key;

        public VCardEncryptor(User User)
        {
            key = System.Convert.FromBase64String(User.Password);
        }

        /// <summary>
        /// Encrypts the token data with Rijndael method.
        /// It returns a Base64 string, with a byte[] enconded were it follows the next pattern:
        /// [IV_Size|IV_Data|Encrypted_Size|Encrypted_Data]
        /// </summary>
        /// <param name="Token">The card to be encrypted.</param>
        /// <returns>The encrypted card.</returns>
        public String RijndaelEncrypt(VCardToken Token)
        {
            String EncryptedData;
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateIV();
                byte[] IV = myRijndael.IV;
                ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);

                byte[] crypted;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Token.Data);
                        }
                        crypted = msEncrypt.ToArray();
                    }
                }
                EncryptedData = System.Convert.ToBase64String(IV) + "|" +
                    System.Convert.ToBase64String(crypted);
            }
            return EncryptedData;
        }

        public String RijndaelDecrypt(String EncryptedData)
        {
            string[] splited = EncryptedData.Split('|');
            byte[] IV = System.Convert.FromBase64String(splited[0]);
            byte[] cryptedCard = System.Convert.FromBase64String(splited[1]);

            String b64CardData;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cryptedCard))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            b64CardData = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return b64CardData;
            }
        }
    }
}