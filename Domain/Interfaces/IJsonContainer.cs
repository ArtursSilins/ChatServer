using Domain.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Interfaces
{
    public interface IJsonContainer
    {
        ObservableCollection<MessageContent> Messages { get; set; }
        List<Person> Persons { get; set; }
        CurrentPersonId CurrentPersonId { get; set; }
    }
}
