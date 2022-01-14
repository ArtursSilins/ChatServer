using Domain.Interfaces.DBOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DBOperations
{
    public static class DBAdd
    {
        public static IDBAdd Add {get;set;}

        public static void User(string name, int sex, bool defautlPic, string email, byte[] salt, byte[] hash)
        {
            Add.User(name, sex, defautlPic, email, salt, hash);
        }
        public static void PublicMessage(string dateAndTime, string userID,
           string message, byte[] messageImage, string defautlPicture)
        {
            Add.PublicMessage(dateAndTime, userID, message, messageImage, defautlPicture);
        }
    }
}
