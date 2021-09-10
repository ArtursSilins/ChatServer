
using Domain.Data;

namespace Domain.Interfaces
{
    public interface IMessageContent
    {
        string Name { get; set; }

        string MessageText { get; set; }

        string MessageTime { get; set; }

        string MessageAlignment { get; set; }

        string MessagePictureVisibility { get; set; }

        string MessageColour { get; set; }

        string MessagePicture { get; set; }
        byte[] Pic { get; set; }

        bool PictureChanged { get; set; }
        int Id { get; set; }

        IMessageContent NewInstance(IMessageContent from);
    }
}
