using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Converters
{
    public static class Hash
    {
        //PBKDF2 pBKDF2 = new PBKDF2("Password",24,90000,"SHA256");
        //pBKDF2.GetBytes(24);

        /// <summary>
        /// Get Salt
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] CreateSalt(int size)
        {
            var salt = new byte[size];
            using(var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
            ////Generate a cryptographic random number.
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //byte[] buff = new byte[size];
            //rng.GetBytes(buff);

            //// Return a Base64 string representation of the random number.
            //return Convert.ToBase64String(buff);
        }
        /// <summary>
        /// Get SaltedHash from Password
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
        /// <summary>
        /// Compare Two Hashed Passwords
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
