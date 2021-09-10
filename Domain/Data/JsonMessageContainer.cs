using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class JsonMessageContainer : IJsonMessageContainer
    {
        public ChatSwitch Switch { get; set; }
        public MessageContent Message { get; set; }
    }
}
