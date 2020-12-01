using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    public static class AllMessages
    {
        public static List<IMessageContent> Messages { get; set; } = new List<IMessageContent>();
    }
}
