using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IJsonMessageContainer
    {
        ChatSwitch Switch { get; set; }
        MessageContent Message { get; set; }
    }
}
