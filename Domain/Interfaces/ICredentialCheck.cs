using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICredentialCheck
    {
        bool Name { get; set; }
        bool Email { get; set; }

        bool UserExists(string name, string password);
        bool UserNameCheck(string name);
        bool EmailExists(string email);
    }
}
