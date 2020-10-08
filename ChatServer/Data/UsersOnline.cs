using ChatServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Data
{
    public static class UsersOnline
    {
        public static List<IPerson> Persons { get; set; } = new List<IPerson>();
    }
}
