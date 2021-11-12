using Domain.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data.KeyContainer
{
    static class KeyList
    {
        /// <summary>
        /// Hold Symmetric and Asymmetric keys for eache user.
        /// </summary>
        public static List<UserKeys> Keys { get; set; } = new List<UserKeys>();
        /// <summary>
        /// Add clients public key to list.
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="pubKey"></param>
        public static void AddPublicKey(string personId, string pubKey)
        {
            Keys.Find(x => x.UserID == personId).PublicAsymmetricKey =
                AsymmetricEncryption.StringToPubKey(pubKey);
        }
        public static RSAParameters GetPublicKey(string personId)
        {
            return Keys.Find(x => x.UserID == personId).PublicAsymmetricKey;
        }
        public static string GetSymmetricKeyString(string personId)
        {
            RijndaelManaged rijndaelManaged = Keys.Find(x => x.UserID == personId).SymmetricKeys;

            string key = Convert.ToBase64String( rijndaelManaged.Key );

            return key;
        }
        public static byte[] GetSymmetricKey(string personId)
        {
            RijndaelManaged rijndaelManaged = Keys.Find(x => x.UserID == personId).SymmetricKeys;

            return rijndaelManaged.Key;
        }

        public static string GetSymmetricIVString(string personId)
        {
            RijndaelManaged rijndaelManaged = Keys.Find(x => x.UserID == personId).SymmetricKeys;

            string IV = Convert.ToBase64String(rijndaelManaged.IV);

            return IV;
        }
        public static byte[] GetSymmetricIV(string personId)
        {
            RijndaelManaged rijndaelManaged = Keys.Find(x => x.UserID == personId).SymmetricKeys;

            return rijndaelManaged.IV;
        }
    }
}
