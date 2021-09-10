using Domain.Converters;
using Domain.Data;
using Domain.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Domain
{
    public class Connect
    {
        private int _PersonId { get; set; }
        private IMessageContent _messageContent;
        private IPerson _person;
        private IJsonContainer _jsonContainer;
        private IJsonMessageContainer _jsonMessageContainer;
        private IChatSwitch _ChatSwitch;
        public IUIViewModel _uIViewModel;
        public static ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        public bool FirstTime { get; set; }

        private bool NotLastRecursion { get; set; }

        public Connect(int personId, IChatSwitch chatSwitch, IPerson person,
            IJsonContainer jsonContainer, IJsonMessageContainer jsonMessageContainer, IMessageContent messageContent, IUIViewModel uIViewModel)
        {
            _PersonId = personId;
            _person = person;
            _ChatSwitch = chatSwitch;
            _jsonContainer = jsonContainer;
            _jsonMessageContainer = jsonMessageContainer;
            _uIViewModel = uIViewModel;
        }
        public void ReadCallback(IAsyncResult ar)
        {

            try
            {

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
 
                int byteCount = handler.EndReceive(ar);


                // If byteCount is 1024 (Max BufferSize = 1024) then there is more to read

                if (byteCount == 1024)
                    NotLastRecursion = true;

                if (byteCount == 0)
                {
                    // Add all disconection stuff !!!!!

                    return;
                }

                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, byteCount));

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
              new AsyncCallback(ReadCallback), state);

                if(NotLastRecursion == false && handler.Available == 0)
                {

                    if (state.sb.Length > 1)
                    {
                        Chat(state.sb.ToString(), handler);
                        state.sb.Clear();
                    }
                    ServerConfig.ReadDone.Set();
                }
                else
                {
                    NotLastRecursion = false;
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        private void Chat(string textFromClient, Socket socket)
        {

            if (FirstTime)
            {
                FirstTime = false;

                _person = ConverData.ToReceiv<Person>(textFromClient);

                _person.PersonId = _PersonId;
                _person.Connection = socket;

                UsersOnline.Persons.Add(_person);

                ShowUsersOnline(_uIViewModel);

                AddPersonsToJsonContainer(_jsonContainer);

                foreach (var user in UsersOnline.Persons)
                {
                    if (user.PersonId == _PersonId)
                    {
                        _jsonContainer.CurrentPersonId.Id = _PersonId;

                        AddMessagesToJsonContainer(_jsonContainer);

                        user.Connection.Send(ConverData.ToSend(_jsonContainer));

                    }
                    else
                    {
                        _jsonContainer.Messages.Clear();
                        user.Connection.Send(ConverData.ToSend(_jsonContainer));
                    }
                }

            }
            else
            {
                DisconnectContent disconnectContent = new DisconnectContent();

                disconnectContent = ConverData.ToReceiv<DisconnectContent>(textFromClient);

                if (disconnectContent.ExitMessage != "€noc§dne§€")
                {
                    _jsonMessageContainer = ConverData.ToReceiv<JsonMessageContainer>(textFromClient);

                    string tempName = _jsonMessageContainer.Message.Name; 

                    AllMessages.Messages.Add(_jsonMessageContainer.Message.NewInstance(_jsonMessageContainer.Message));

                    AddToMessagesOnServer();

                    foreach (var item in UsersOnline.Persons)
                    {
                        if (item.PersonId == _jsonMessageContainer.Message.Id)
                        {
                            _jsonMessageContainer.Message.MessageAlignment = "Right";
                            _jsonMessageContainer.Message.MessageColour = SenderReceiwer.SendBubbleColor;
                            _jsonMessageContainer.Message.MessagePictureVisibility = "Hidden";
                            _jsonMessageContainer.Message.Name = "";
                        }
                        else
                        {
                            _jsonMessageContainer.Message.MessageAlignment = "Left";
                            _jsonMessageContainer.Message.MessageColour = SenderReceiwer.ReceiveBubleColor;
                            _jsonMessageContainer.Message.MessagePictureVisibility = "Visible";
                            _jsonMessageContainer.Message.Name = tempName;
                        }

                        item.Connection.Send(ConverData.ToSend(_jsonMessageContainer));
                    }


                }
                else
                {
                    RemoveDisconnectedUser(disconnectContent);

                    SendUsersOnlineToClients();

                    ShowUsersOnline(_uIViewModel);

                }
            }

        }
        /// <summary>
        /// Only for server
        /// </summary>
        private void AddToMessagesOnServer()
        {
            _uIViewModel.ChatMessages += _jsonMessageContainer.Message.Name + "\n" +
               _jsonMessageContainer.Message.MessageText + "\n" +
               _jsonMessageContainer.Message.MessageTime + "\n\n";
        }

        private void RemoveDisconnectedUser(DisconnectContent disconnectContent)
        {
            _jsonContainer.Persons.Clear();

            UsersOnline.Persons.RemoveAll(x => x.PersonId == disconnectContent.Id);

            foreach (var item in UsersOnline.Persons)
            {
                Person temp = new Person();

                temp.Connection = null;
                temp.Female = item.Female;
                temp.Male = item.Male;
                temp.Name = item.Name;
                temp.PersonId = item.PersonId;
                temp.Pic = item.Pic;
                temp.PicturePath = @"C:\Users\X\Downloads\ChatData\ChatImage" + item.PersonId + ".jpg";

                _jsonContainer.Persons.Add(temp);
            }
           
        }

        private void SendUsersOnlineToClients()
        {
            foreach (var item in UsersOnline.Persons)
            {
                item.Connection.Send(ConverData.ToSend(_jsonContainer));
            }
        }

        /// <summary>
        /// Only for server
        /// </summary>
        /// <param name="uIViewModel"></param>
        private void ShowUsersOnline(IUIViewModel uIViewModel)
        {
            uIViewModel.UsersOnline = "";
            foreach (var item in UsersOnline.Persons)
            {
                uIViewModel.UsersOnline += item.Name + "\n";
            }
        }
      
        private void AddPersonsToJsonContainer(IJsonContainer jsonContainer)
        {
            foreach (var item in UsersOnline.Persons)
            {
                Person temp = new Person();

                temp.Connection = null;
                temp.Female = item.Female;
                temp.Male = item.Male;
                temp.Name = item.Name;
                temp.PersonId = item.PersonId;
                temp.Pic = item.Pic;
                temp.PicturePath = @"C:\Users\X\Downloads\ChatData\ChatImage" + item.PersonId + ".jpg";

                jsonContainer.Persons.Add(temp);
            }
        }
        private void AddMessagesToJsonContainer(IJsonContainer jsonContainer)
        {
            foreach (var item in AllMessages.Messages)
            {
                MessageContent messageContent = new MessageContent();

                messageContent.Id = item.Id;
                messageContent.MessageAlignment = "left";
                messageContent.MessageColour = SenderReceiwer.ReceiveBubleColor;
                messageContent.MessagePicture = item.MessagePicture;
                messageContent.MessagePictureVisibility = "visible";
                messageContent.MessageText = item.MessageText;
                messageContent.MessageTime = item.MessageTime;
                messageContent.Name = item.Name;
                messageContent.Pic = item.Pic;
                messageContent.PictureChanged = item.PictureChanged;

                jsonContainer.Messages.Add(messageContent);

            }
        }
    }
}
