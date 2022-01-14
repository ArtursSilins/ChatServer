using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.DBOperations
{
    public interface IDBGet
    {
        DataTable PublicMessages();

        byte[] Salt(string name);
       
        int UserGender(string name);
       
    }
}
