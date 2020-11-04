using ChatServer.Converters;
using ChatServer.Data;
using ChatServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Connect
    {
        private int _PersonId { get; set; }
        private IMessageContent _messageContent;
        private IPerson _person;
        private IJsonContainer _jsonContainer;
        public IUIViewModel _uIViewModel;

        public Connect(int personId, IPerson person, IJsonContainer jsonContainer, IMessageContent messageContent, IUIViewModel uIViewModel)
        {
            _PersonId = personId;
            _person = person;
            _jsonContainer = jsonContainer;
            _uIViewModel = uIViewModel;
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);           

            byte[] dataBuffer = new byte[received];
            Array.Copy(MainWindow.buffer, dataBuffer, received);
            string textFromClient = Encoding.UTF8.GetString(dataBuffer);

            Chat(textFromClient, socket, _person, _jsonContainer, _uIViewModel);

        }
        private void Chat(string textFromClient, Socket socket, IPerson person, IJsonContainer jsonContainer, IUIViewModel uIViewModel)
        {
            bool Connected = true;
            bool FirstTime = true;


            if (textFromClient == "Connected")
            {
                while (Connected)
                {
                    var textFromClient1 = ReceivData(socket);
                                       

                    if (FirstTime)
                    {
                        FirstTime = false;

                        person = ConverData.ToReceiv<Person>(textFromClient1);

                        person.PersonId = _PersonId;
                        person.Connection = socket;
                                                
                        UsersOnline.Persons.Add(person);

                        ShowUsersOnline(uIViewModel);

                        AddPersonsToJsonContainer(jsonContainer);

                        foreach (var user in UsersOnline.Persons)
                        {
                            if(user.PersonId == _PersonId)
                            {
                                jsonContainer.CurrentPersonId.Id = _PersonId;

                                AddMessagesToJsonContainer(jsonContainer);
                               
                                user.Connection.Send(ConverData.ToSend(jsonContainer));

                            }
                            else
                            {
                                jsonContainer.Messages.Clear();
                                user.Connection.Send(ConverData.ToSend(jsonContainer));
                            }
                        }
                        
                    }
                    else
                    {
                        DisconnectContent disconnectContent = new DisconnectContent();

                        disconnectContent = ConverData.ToReceiv<DisconnectContent>(textFromClient1);
                        if (disconnectContent.ExitMessage != "€noc§dne§€")
                        {
                            _messageContent = ConverData.ToReceiv<MessageContent>(textFromClient1);

                            string tempName = _messageContent.Name;

                            AllMessages.Messages.Add(_messageContent.NewInstance(_messageContent));


                            uIViewModel.ChatMessages += _messageContent.Name + "\n" +
                               _messageContent.MessageText + "\n" +
                               _messageContent.MessageTime + "\n\n";

                            foreach (var item in UsersOnline.Persons)
                            {
                                if(item.PersonId == _messageContent.Id)
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
                            jsonContainer.Persons.Clear();

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

                                jsonContainer.Persons.Add(temp);
                            }

                            foreach (var item in UsersOnline.Persons)
                            {                            
                                item.Connection.Send(ConverData.ToSend(jsonContainer));
                            }

                            ShowUsersOnline(uIViewModel);
                           
                            Connected = false;
                        }
                    }
                }
            }

           
        }
        private void ShowUsersOnline(IUIViewModel uIViewModel)
        {
            uIViewModel.UsersOnline = "";
            foreach (var item in UsersOnline.Persons)
            {
                uIViewModel.UsersOnline += item.Name + "\n";
            }
        }
        private string ReceivData(Socket socket)
        {

            var buffer = new byte[256];

            int size = 0;

            var textFromServer = new StringBuilder();

            do
            {
                size = socket.Receive(buffer);

                textFromServer.Append(Encoding.UTF8.GetString(buffer, 0, size));

            } while (socket.Available > 0);

            return textFromServer.ToString();
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
