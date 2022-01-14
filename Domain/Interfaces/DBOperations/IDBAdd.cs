using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.DBOperations
{
    public interface IDBAdd
    {
        void User(string name, int sex, bool defautlPic, string email, byte[] salt, byte[] hash);
        void PublicMessage(string dateAndTime, string userID,
           string message, byte[] messageImage, string defautlPicture);
    }
}
