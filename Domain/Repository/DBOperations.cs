using Domain.Interfaces.Repository;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class DBOperations : IDBOperations
    {
        public DBOperationsRepository DB { get; set; } = new DBOperationsRepository();

        public void Add(string userId, string name, string email, string salt, string hash)
        {
            DB.Add(userId, name, email, salt, hash);
        }
    }
}
