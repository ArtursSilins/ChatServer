using System.Net.Sockets;

namespace Domain.Interfaces
{
    public interface IPerson
    {
        string Name { get; set; }
        bool Male { get; set; }
        bool Female { get; set; }
        Socket Connection { get; set; }
        int PersonId { get; set; }
        string PicturePath { get; set; }
        byte[] Pic { get; set; }
    }
}
