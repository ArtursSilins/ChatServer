using Domain.Converters;
using Domain.Data;
using Domain.Data.KeyContainer;
using Domain.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Domain
{
    public class Connect
    {
        private string _PersonId { get; set; }
        private IMessageContent _messageContent;
        private IPerson _person;
        private IJsonBaseContainer _jsonContainer;
        private IJsonMessageContainer _jsonMessageContainer;
        private IChatSwitch _ChatSwitch;
        public IUIViewModel _uIViewModel;
        private ICredential _Credential;
        public static ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        public bool FirstTimeConnect { get; set; }
        public bool FirstTime { get; set; }

        private bool NotLastRecursion { get; set; }

        public Connect(string personId, IChatSwitch chatSwitch, IPerson person,
            IJsonBaseContainer jsonContainer, IJsonMessageContainer jsonMessageContainer,
            IMessageContent messageContent, IUIViewModel uIViewModel, ICredential credential)
        {
            _PersonId = personId;
            _person = person;
            _ChatSwitch = chatSwitch;
            _jsonContainer = jsonContainer;
            _jsonMessageContainer = jsonMessageContainer;
            _uIViewModel = uIViewModel;
            _Credential = credential;
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
                        ReceivData(state.sb.ToString(), handler);
                        //Chat(state.sb.ToString(), handler);
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

        private void ReceivData(string textFromClient, Socket socket)
        {
            if (FirstTimeConnect)
            {
                _Credential = ConvertData.FirstTimeReceiv<Credential>(textFromClient);
                FirstTimeConnect = false;
            }
            else
            {

                _jsonContainer = ConvertData.ToReceive<BaseContainer>(textFromClient, _PersonId);
            }

            if (_Credential.NeedAction || _jsonContainer.Credential.NeedAction)
            {
                if (_Credential.NeedKeys)
                {
                    AsymmetricEncryption.CreateKey(_PersonId);

                    SymmetricEncryption.GenerateKey(_PersonId);

                    KeyList.AddPublicKey(_PersonId, _Credential.PubKey);

                    _Credential.PubKey = AsymmetricEncryption.PubKeyString(_PersonId);

                    _Credential.SymmetricKey = AsymmetricEncryption.EncryptForPrivateKey(KeyList.GetPublicKey(_PersonId),
                        KeyList.GetSymmetricKeyString(_PersonId));

                    _Credential.IV = AsymmetricEncryption.EncryptForPrivateKey(KeyList.GetPublicKey(_PersonId),
                        KeyList.GetSymmetricIVString(_PersonId));

                    //send Keys to client with clients public key encryption
                    socket.Send(ConvertData.ToSend( _Credential ));  
                }

                if (_jsonContainer.Credential.SignIn)
                {

                }

                if (_jsonContainer.Credential.Login)
                {


                    if (LoginSuccess())
                    {
                        //KeyList.Keys.Find(x=>x.UserID == _PersonId).UserID = //sql
                        //_PersonId = 
                    }
                }
            }
            else
            {
                Chat(textFromClient, socket);
            }
        }

        private void Chat(string textFromClient, Socket socket)
        {

            if (FirstTime)
            {
                FirstTime = false;

                _person = ConvertData.ToReceive<Person>(textFromClient, _PersonId);

                _person.PersonId = _PersonId;
                _person.Connection = socket;

                UsersOnline.Persons.Add(_person);

                ShowUsersOnline(_uIViewModel);

                AddPersonsToJsonContainer(_jsonContainer);

                foreach (var user in UsersOnline.Persons)
                {
                    if (user.PersonId == _PersonId)
                    {
                        _jsonContainer.CurrentPersonId = _PersonId;

                        AddMessagesToJsonContainer(_jsonContainer);

                        user.Connection.Send(ConvertData.ToSend(_jsonContainer));

                    }
                    else
                    {
                        _jsonContainer.Messages.Clear();
                        user.Connection.Send(ConvertData.ToSend(_jsonContainer));
                    }
                }

            }
            else
            {
                DisconnectContent disconnectContent = new DisconnectContent();

                disconnectContent = ConvertData.ToReceive<DisconnectContent>(textFromClient, _PersonId);

                if (disconnectContent.ExitMessage != "€noc§dne§€")
                {
                    _jsonMessageContainer = ConvertData.ToReceive<MessageContainer>(textFromClient, _PersonId);

                    string tempName = _jsonMessageContainer.Message.Name;

                    AllMessages.Messages.Add(_jsonMessageContainer.Message.NewInstance(_jsonMessageContainer.Message));

                    AddToMessagesOnServer();

                    if(_jsonMessageContainer.Switch.ChatMode == ChatMode.Public)
                    {
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

                            item.Connection.Send(ConvertData.ToSend(_jsonMessageContainer));
                        }
                    }else if(_jsonMessageContainer.Switch.ChatMode == ChatMode.Private)
                    {
                        foreach (var item in UsersOnline.Persons)
                        {

                            if(_jsonMessageContainer.Message.IdList.Exists(x=>x == item.PersonId))
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

                                item.Connection.Send(ConvertData.ToSend(_jsonMessageContainer));
                            }
                        }
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
                item.Connection.Send(ConvertData.ToSend(_jsonContainer));
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
      
        private void AddPersonsToJsonContainer(IJsonBaseContainer jsonContainer)
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
        private void AddMessagesToJsonContainer(IJsonBaseContainer jsonContainer)
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
                messageContent.IdList = item.IdList;

                jsonContainer.Messages.Add(messageContent);

            }
        }
    }
}
