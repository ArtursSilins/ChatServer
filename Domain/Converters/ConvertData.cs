using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Converters
{
    public static class ConverData
    {
        public static byte[] ToSend(object data)
        {
            string DataToSend = JsonConvert.SerializeObject(data);
            byte[] DataInBytes = Encoding.UTF8.GetBytes(MessageEncryption.Encrypt(DataToSend));

            return DataInBytes;
        }
        public static T ToReceiv<T>(string textFromServer)
        {
            T objectFromText = JsonConvert.DeserializeObject<T>(MessageEncryption.Decrypt(textFromServer));

            return objectFromText;
        }
    }
}
