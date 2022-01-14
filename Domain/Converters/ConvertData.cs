using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Converters
{
    public static class ConvertData
    {
        public static byte[] ToSend(object data)
        {
            string DataToSend = JsonConvert.SerializeObject(data);
            byte[] DataInBytes = Encoding.UTF8.GetBytes(DataToSend);

            return DataInBytes;
        }
        public static byte[] ToSend(object data, string personId)
        {
            string DataToSend = JsonConvert.SerializeObject(data);
            byte[] DataInBytes = Encoding.UTF8.GetBytes(SymmetricEncryption.EncryptDataToBytes(DataToSend, personId));

            return DataInBytes;
        }

        public static T ToReceive<T>(string text, string personId)
        {
            T objectFromText = JsonConvert.DeserializeObject<T>(SymmetricEncryption.Decrypt(text, personId));

            return objectFromText;
        }
        public static T FirstTimeReceiv<T>(string textFromServer)
        {
            T objectFromText = JsonConvert.DeserializeObject<T>(textFromServer);

            return objectFromText;
        }
    }
}
