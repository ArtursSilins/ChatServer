using Domain.Converters;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class CredentialCheck:ICredentialCheck
    {
        public bool Name { get; set; }
        public bool Email { get; set; }
        /// <summary>
        /// Checking if user name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UserExists(string name, string password)
        {
            bool exists = false;

            byte[] salt = DBOperations.DBGet.Salt(name);

            //if salt is null user not registered or user name incorret.
            if (salt == null)
                return exists;
            else
                exists = DBOperations.DBCheck.UseerExists(name,
                    Hash.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), salt));

            return exists;
        }
        /// <summary>
        /// Check if user name is taken.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UserNameCheck(string name)
        {
            bool exists = false;

            exists = DBOperations.DBCheck.UseerNameExists(name);

            Name = exists;

            return exists;
        }
        /// <summary>
        /// Check if email is taken.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool EmailExists(string email)
        {
            bool exists = false;

            string tempEmail = email.ToLower();

            exists = DBOperations.DBCheck.EmailExists(email);

            Email = exists;

            return exists;
        }
    }
}
