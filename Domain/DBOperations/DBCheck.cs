using Domain.Interfaces.DBOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DBOperations
{
    public static class DBCheck
    {
        public static IDBCheck Check {get;set; }

        public static bool UseerExists(string name, byte[] hash)
        {
            return Check.UserExists(name, hash);
        }
        public static bool UseerNameExists(string name)
        {
            return Check.UserNameExists(name);
        }
        public static bool EmailExists(string email)
        {
            return Check.EmailExists(email);
        }
    }
}
