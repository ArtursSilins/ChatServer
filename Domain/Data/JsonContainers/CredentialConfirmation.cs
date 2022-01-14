using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data.JsonContainers
{
    public class CredentialConfirmation : ICredentialConfirmation
    {
        public bool Status { get; set; }
        public bool Name { get; set; }
        public bool Email { get; set; }
        public bool Login { get; set; }
        public bool SignIn { get; set; }
    }
}
