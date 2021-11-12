using Domain.Data.KeyContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Converters
{
    public static class SymmetricEncryption
    {
        /// <summary>
        /// Generate Symmetric key and store it in the list of users keys.
        /// </summary>
        /// <param name="userID"></param>
        public static void GenerateKey(string userID)
        {
            // Create a new instance of the RijndaelManaged
            // class.  This generates a new key and initialization
            // vector (IV).
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();

            UserKeys keyData = new UserKeys();
            keyData.SymmetricKeys = myRijndael;
            keyData.UserID = userID;

            KeyList.Keys.Add(keyData);
        }

        public static byte[] EncryptStringToBytes(string plainText, string personId)
        {
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = KeyList.GetSymmetricKey(personId);
                rijAlg.IV = KeyList.GetSymmetricIV(personId);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string Decrypt(string text, string personId)
        {

            byte[] data = Convert.FromBase64String(text);

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = KeyList.GetSymmetricKey(personId);
                rijAlg.IV = KeyList.GetSymmetricIV(personId);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            try
                            {

                            plaintext = srDecrypt.ReadToEnd();
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
