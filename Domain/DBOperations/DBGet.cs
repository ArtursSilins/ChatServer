using Domain.Interfaces.DBOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DBOperations
{
    public static class DBGet
    {
        public static IDBGet Get { get; set; }

        public static DataTable PublicMessage()
        {
            return Get.PublicMessages();
        }
        public static byte[] Salt(string name)
        {
            return Get.Salt(name);
        }
        public static int UserGender(string name)
        {
            return Get.UserGender(name);
        }
    }
}
