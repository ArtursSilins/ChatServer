using Domain.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class BaseContainer : IJsonBaseContainer
    {
        public ObservableCollection<MessageContent> Messages { get; set; }
        public List<Person> Persons { get; set; }
        public string CurrentPersonId { get; set; }
        public Credential Credential { get; set; }
    }
}
