using ChatServer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IJsonContainer
    {
        ObservableCollection<MessageContent> Messages { get; set; }
        List<Person> Persons { get; set; }
        CurrentPersonId CurrentPersonId { get; set; }
    }
}
