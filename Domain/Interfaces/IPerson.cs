using System.Net.Sockets;

namespace Domain.Interfaces
{
    public interface IPerson
    {
        string Name { get; set; }
        int Sex { get; set; }
        Socket Connection { get; set; }
        string PersonId { get; set; }
        string PicturePath { get; set; }
        byte[] Pic { get; set; }
    }
}
