using Domain.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Interfaces
{
    public interface IJsonBaseContainer
    {
        ObservableCollection<MessageContent> Messages { get; set; }
        List<Person> Persons { get; set; }
        string CurrentPersonId { get; set; }
        Credential Credential { get; set; }
    }
}
