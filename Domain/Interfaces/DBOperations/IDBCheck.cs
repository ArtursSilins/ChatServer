using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.DBOperations
{
    public interface IDBCheck
    {
        /// <summary>
        /// Compare name and hashed password.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        bool UserExists(string name, byte[] hash);
        bool UserNameExists(string name);
        bool EmailExists(string email);
    }
}
