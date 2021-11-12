using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data.KeyContainer
{
    public class UserKeys
    {
        public RijndaelManaged SymmetricKeys { get; set; }
        public RSAParameters PublicAsymmetricKey { get; set; }
        public string UserID { get; set; }
    }
}
