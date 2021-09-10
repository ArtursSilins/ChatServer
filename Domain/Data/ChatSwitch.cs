using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    public enum ChatMode
    {
        Public = 0,
        Private = 1
    }
    public class ChatSwitch:IChatSwitch
    {
        public ChatMode ChatMode { get; set; }

    }
}
