using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IUIViewModel
    {
        string ChatMessages { get; set; }
        string UsersOnline { get; set; }
    }
}
