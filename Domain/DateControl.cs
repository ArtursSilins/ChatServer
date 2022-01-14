using Domain.Converters;
using Domain.Data;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Add date between messages.
    /// </summary>
    public static class DateControl
    {
        /// <summary>
        /// Add dates between messages when client first time getting messages from DB.
        /// </summary>
        public static DateTime CheckDateOnFirstLoad(DateTime tempDateTime, DateTime getDateTime,
            DataRow item, IJsonBaseContainer jsonContainer)
        {
            if (tempDateTime.Date < getDateTime.Date)
            {
                tempDateTime = getDateTime;

                MessageContent messageContent = new MessageContent();

                messageContent.Id = null;
                messageContent.MessageAlignment = "Center";
                messageContent.MessageColour = "#adbab1";

                messageContent.MessagePicture = item["DefaultPicture"].ToString();
                messageContent.MessagePictureVisibility = "Collapsed";
                messageContent.MessageText = tempDateTime.ToString("dddd, dd MMMM yyyy");
                messageContent.MessageTime = "";
                messageContent.Name = "";
                messageContent.Pic = null;
                messageContent.PictureChanged = false;
                messageContent.IdList = new List<string>();

                jsonContainer.Messages.Add(messageContent);
            }

            return tempDateTime;
        }
        public static void CheckForDateChange(DateTime dateTime, DateTime currentDate, IPerson person, IJsonMessageContainer _jsonMessageContainer)
        {
            if (dateTime.Date > currentDate.Date)
            {
                currentDate = dateTime;

                IJsonMessageContainer jsonMessageContainer = new MessageContainer();
                jsonMessageContainer.Message = new MessageContent();

                jsonMessageContainer.Switch = new ChatSwitch();
                jsonMessageContainer.Switch.ChatMode = _jsonMessageContainer.Switch.ChatMode;

                MessageContent messageContent = new MessageContent();

                jsonMessageContainer.Message.Id = null;
                jsonMessageContainer.Message.MessageAlignment = "Center";
                jsonMessageContainer.Message.MessageColour = "#adbab1";

                jsonMessageContainer.Message.MessagePicture = "/View/Images/Male.jpg";
                jsonMessageContainer.Message.MessagePictureVisibility = "Collapsed";
                jsonMessageContainer.Message.MessageText = currentDate.ToString("dddd, dd MMMM yyyy");
                jsonMessageContainer.Message.MessageTime = "";
                jsonMessageContainer.Message.Name = "";
                jsonMessageContainer.Message.Pic = null;
                jsonMessageContainer.Message.PictureChanged = false;
                jsonMessageContainer.Message.IdList = new List<string>();

                person.Connection.Send(ConvertData.ToSend(jsonMessageContainer, person.PersonId));
            }
        }
    }
}
