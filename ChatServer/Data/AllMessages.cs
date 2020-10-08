using ChatServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Data
{
    public static class AllMessages
    {
        public static List<IMessageContent> Messages { get; set; } = new List<IMessageContent>();
    }
}
