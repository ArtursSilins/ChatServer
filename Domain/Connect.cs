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
        public IUIViewModel _uIViewModel;
        public static ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        public bool FirstTime { get; set; }

        private bool LastRecursion { get; set; }

        public Connect(int personId, IPerson person, IJsonContainer jsonContainer, IMessageContent messageContent, IUIViewModel uIViewModel)
        {
            _PersonId = personId;
            _person = person;
            _jsonContainer = jsonContainer;
            _uIViewModel = uIViewModel;
        }
        public void ReadCallback(IAsyncResult ar)
        {

            try
            {

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
 
                int read = handler.EndReceive(ar);

                if (read == 1024)
                    LastRecursion = true;

                if (read == 0)
                {
                    // Add all disconection stuff !!!!!

                    return;
                }

                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, read));

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
              new AsyncCallback(ReadCallback), state);

                if(LastRecursion == false && handler.Available == 0)
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
                    LastRecursion = false;
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
                    _messageContent = ConverData.ToReceiv<MessageContent>(textFromClient);

                    string tempName = _messageContent.Name;

                    AllMessages.Messages.Add(_messageContent.NewInstance(_messageContent));

                    AddToMessagesOnServer();

                    foreach (var item in UsersOnline.Persons)
                    {
                        if (item.PersonId == _messageContent.Id)
                        {
                            _messageContent.MessageAlignment = "Right";
                            _messageContent.MessageColour = SenderReceiwer.SendBubbleColor;
                            _messageContent.MessagePictureVisibility = "Hidden";
                            _messageContent.Name = "";
                        }
                        else
                        {
                            _messageContent.MessageAlignment = "Left";
                            _messageContent.MessageColour = SenderReceiwer.ReceiveBubleColor;
                            _messageContent.MessagePictureVisibility = "Visible";
                            _messageContent.Name = tempName;
                        }

                        item.Connection.Send(ConverData.ToSend(_messageContent));
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
            _uIViewModel.ChatMessages += _messageContent.Name + "\n" +
               _messageContent.MessageText + "\n" +
               _messageContent.MessageTime + "\n\n";
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
