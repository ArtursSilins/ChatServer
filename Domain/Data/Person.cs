using Domain.Interfaces;
using System.Net.Sockets;

namespace Domain.Data
{
    public class Person : IPerson
    {
        public string Name { get; set; }
        public bool Male { get; set; }
        public bool Female { get; set; }
        public Socket Connection { get; set; }
        public int PersonId { get; set; }
        public string PicturePath { get; set; }
        public byte[] Pic { get; set; }

    }
}
