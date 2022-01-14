using Domain.Converters;
using Domain.Data;
using Domain.Data.KeyContainer;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Domain
{
    public class Connect
    {
        public ICredentialConfirmation _CredentialConfirmation { get; private set; }
        private string _PersonId { get; set; }
        private IMessageContent _messageContent;
        private IPerson _person;
        private IJsonBaseContainer _jsonContainer;
        private IJsonMessageContainer _jsonMessageContainer;
        private IChatSwitch _ChatSwitch;
        public IUIViewModel _uIViewModel;
        private ICredential _Credential;
        private ICredentialCheck _credentialCheck;

        public static ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        public bool FirstTimeConnect { get; set; }
        public bool FirstTime { get; set; }

        private bool NotLastRecursion { get; set; }
        public bool ChatEnabled { get; private set; }
        public bool ChatRun { get; private set; }
        /// <summary>
        /// Holds Current Message date.
        /// </summary>
        public DateTime CurrentDate { get; set; }

        public Connect(string personId, IChatSwitch chatSwitch, IPerson person,
            IJsonBaseContainer jsonContainer, IJsonMessageContainer jsonMessageContainer,
            IMessageContent messageContent, IUIViewModel uIViewModel, ICredential credential,
            ICredentialConfirmation credentialConfirmation, ICredentialCheck credentialCheck)
        {
            _CredentialConfirmation = credentialConfirmation;
            _PersonId = personId;
            _person = person;
            _ChatSwitch = chatSwitch;
            _jsonContainer = jsonContainer;
            _jsonMessageContainer = jsonMessageContainer;
            _uIViewModel = uIViewModel;
            _Credential = credential;
            _credentialCheck = credentialCheck;
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
                if (!textFromClient.Contains("NeedAction")) return;

                _Credential = ConvertData.FirstTimeReceiv<Credential>(textFromClient);
                FirstTimeConnect = false;
            }
            else
            {

                _jsonContainer = ConvertData.ToReceive<BaseContainer>(textFromClient, _PersonId);
            }

            GetKeys(socket);

            LogInAndSignIn(socket);


            if (ChatEnabled)
                Chat(textFromClient, socket);

        }

        private void LogInAndSignIn(Socket socket)
        {
            if (_jsonContainer.Credential == null) return;

            if (_jsonContainer.Credential.NeedAction)
            {
                if (_jsonContainer.Credential.SignIn)
                {

                    _CredentialConfirmation.SignIn = true;
                    _CredentialConfirmation.Login = false;

                    if (!_credentialCheck.UserNameCheck(_jsonContainer.Credential.UserName) &&
                        !_credentialCheck.EmailExists(_jsonContainer.Credential.Email))
                    {
                        byte[] salt = Hash.CreateSalt(32);

                        bool defaultPic = true;

                        DBOperations.DBAdd.User(_jsonContainer.Credential.UserName,
                            _jsonContainer.Credential.Sex,
                            defaultPic,
                            _jsonContainer.Credential.Email,
                            salt,
                            Hash.GenerateSaltedHash(Encoding.UTF8.GetBytes(_jsonContainer.Credential.Password), salt));

                        _CredentialConfirmation.Status = true;
                        socket.Send(ConvertData.ToSend(_CredentialConfirmation, _PersonId));
                    }
                    else
                    {
                        ChatEnabled = false;

                        _CredentialConfirmation.Status = ChatEnabled;

                        _CredentialConfirmation.Name = _credentialCheck.Name;
                        _CredentialConfirmation.Email = _credentialCheck.Email;

                        socket.Send(ConvertData.ToSend(_CredentialConfirmation, _PersonId));
                    }
                }

                if (_jsonContainer.Credential.Login)
                {

                    if (_credentialCheck.UserExists(_jsonContainer.Credential.UserName, _jsonContainer.Credential.Password))
                    {
                        KeyList.Keys.Find(x => x.UserID == _PersonId).UserID = _jsonContainer.Credential.UserName;
                        _PersonId = _jsonContainer.Credential.UserName;

                        ChatEnabled = true;
                    }

                    _CredentialConfirmation.Status = ChatEnabled;
                    _CredentialConfirmation.Login = true;
                    _CredentialConfirmation.SignIn = false;

                    socket.Send(ConvertData.ToSend(_CredentialConfirmation, _PersonId));

                }
            }
        }

        private void GetKeys(Socket socket)
        {
            if (_Credential == null) return;

            if (_Credential.NeedAction)
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

                    //send Keys to client 
                    socket.Send(ConvertData.ToSend(_Credential));

                    _Credential = null;
                }
            }
        }


        private void Chat(string textFromClient, Socket socket)
        {

            if (FirstTime)
            {
                FirstTime = false;

                _person.PersonId = _PersonId;
                _person.Connection = socket;
                _person.Sex = DBOperations.DBGet.UserGender(_PersonId);

                UsersOnline.Persons.Add(_person);

                ShowUsersOnline(_uIViewModel);

                AddPersonsToJsonContainer(_jsonContainer);

                _jsonContainer.Messages = new ObservableCollection<MessageContent>();

                foreach (var user in UsersOnline.Persons)
                {
                    if (user.PersonId == _PersonId)
                    {
                        _jsonContainer.CurrentPersonId = _PersonId;

                        AddMessagesToJsonContainer(_jsonContainer);

                        user.Connection.Send(ConvertData.ToSend(_jsonContainer, _PersonId));
                    }
                    else
                    {
                        _jsonContainer.Messages.Clear();
                        user.Connection.Send(ConvertData.ToSend(_jsonContainer, user.PersonId));
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

                    string tempName = _jsonMessageContainer.Message.Name; ///  ????????????????  ///

                    AllMessages.Messages.Add(_jsonMessageContainer.Message.NewInstance(_jsonMessageContainer.Message));

                    AddToMessagesOnServer();

                    if(_jsonMessageContainer.Switch.ChatMode == ChatMode.Public)
                    {

                        DBOperations.DBAdd.PublicMessage(_jsonMessageContainer.Message.MessageTime,
                            _jsonMessageContainer.Message.Id,
                            _jsonMessageContainer.Message.MessageText,
                            _jsonMessageContainer.Message.Pic,
                            _jsonMessageContainer.Message.MessagePicture);

                            DateTime dateTime = Convert.ToDateTime(_jsonMessageContainer.Message.MessageTime);
                            _jsonMessageContainer.Message.MessageTime = dateTime.ToString("HH:mm");

                        foreach (var item in UsersOnline.Persons)
                        {
                            DateControl.CheckForDateChange(dateTime, CurrentDate, item, _jsonMessageContainer);

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

                            item.Connection.Send(ConvertData.ToSend(_jsonMessageContainer, item.PersonId));
                        }
                    }else if(_jsonMessageContainer.Switch.ChatMode == ChatMode.Private)
                    {
                        foreach (var item in UsersOnline.Persons)
                        {
                            DateTime dateTime = Convert.ToDateTime(_jsonMessageContainer.Message.MessageTime);
                            _jsonMessageContainer.Message.MessageTime = dateTime.ToString("HH:mm");

                            if (_jsonMessageContainer.Message.IdList.Exists(x=>x == item.PersonId))
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

                                item.Connection.Send(ConvertData.ToSend(_jsonMessageContainer, item.PersonId));
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
            _jsonContainer?.Persons?.Clear();

        //    static class SocketExtensions
        //{
        //    public static bool IsConnected(this Socket socket)
        //    {
        //        try
        //        {
        //            return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        //        }
        //        catch (SocketException) { return false; }
        //    }
        //}

        //UsersOnline.Persons.Find(x => x.PersonId == disconnectContent.Id).Connection.BeginDisconnect
        //    (false, new AsyncCallback(DisconnectCallback), UsersOnline.Persons.Find(x => x.PersonId == disconnectContent.Id).Connection);

        //UsersOnline.Persons.Find(x => x.PersonId == disconnectContent.Id).Connection.Shutdown(SocketShutdown.Both);
        //UsersOnline.Persons.Find(x => x.PersonId == disconnectContent.Id).Connection.Close();

        UsersOnline.Persons.RemoveAll(x => x.PersonId == disconnectContent.Id);

            KeyList.Keys.RemoveAll(x => x.UserID == disconnectContent.Id);

            ChatEnabled = false;

            _jsonContainer.Persons = new List<Person>();

            foreach (var item in UsersOnline.Persons)
            {
                Person temp = new Person();

                temp.Connection = null;
                temp.Sex = item.Sex;
                temp.Name = item.Name;
                temp.PersonId = item.PersonId;
                temp.Pic = item.Pic;
                temp.PicturePath = @"C:\Users\X\Downloads\ChatData\ChatImage" + item.PersonId + ".jpg";

                _jsonContainer.Persons.Add(temp);
            }
           
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
        }

        private void SendUsersOnlineToClients()
        {
            foreach (var item in UsersOnline.Persons)
            {
                item.Connection.Send(ConvertData.ToSend(_jsonContainer, item.PersonId));
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
            jsonContainer.Persons = new List<Person>();

            foreach (var item in UsersOnline.Persons)
            {
                Person temp = new Person();

                temp.Connection = null;
                temp.Sex = item.Sex;
                temp.Name = item.Name;
                temp.PersonId = item.PersonId;
                temp.Pic = item.Pic;
                temp.PicturePath = @"C:\Users\X\Downloads\ChatData\ChatImage" + item.PersonId + ".jpg";

                jsonContainer.Persons.Add(temp);
            }
        }
        private void AddMessagesToJsonContainer(IJsonBaseContainer jsonContainer)
        {
            CurrentDate = new DateTime();

            DateTime tempDateTime = new DateTime();

            DateTime getDateTime = new DateTime();
                       

            foreach (DataRow item in DBOperations.DBGet.PublicMessage().Rows)
            {
                getDateTime = Convert.ToDateTime(item["DateAndTime"].ToString());

                tempDateTime = DateControl.CheckDateOnFirstLoad(tempDateTime, getDateTime, item, jsonContainer);

                if (item["UserId"].ToString() == _PersonId)
                {
                    MessageContent messageContent = new MessageContent();

                    messageContent.Id = item["UserId"].ToString();
                    messageContent.MessageAlignment = "Right";
                    messageContent.MessageColour = SenderReceiwer.SendBubbleColor;
                    if (item["DefaultPicture"].ToString() != "")
                        messageContent.MessagePicture = item["DefaultPicture"].ToString();
                    else
                        messageContent.MessagePicture = "";
                    messageContent.MessagePictureVisibility = "Hidden";
                    messageContent.MessageText = item["Message"].ToString();

                    DateTime dateTime = Convert.ToDateTime(item["DateAndTime"].ToString());
                    messageContent.MessageTime = dateTime.ToString("HH:mm");

                    messageContent.Name = ""/*item["UserId"].ToString()*/;
                    messageContent.Pic = null;
                    messageContent.PictureChanged = false;
                    messageContent.IdList = new List<string>();

                    jsonContainer.Messages.Add(messageContent);
                }
                else
                {
                    MessageContent messageContent = new MessageContent();

                    messageContent.Id = item["UserId"].ToString();
                    messageContent.MessageAlignment = "Left";
                    messageContent.MessageColour = SenderReceiwer.ReceiveBubleColor;
                    if (item["DefaultPicture"].ToString() != "")
                        messageContent.MessagePicture = item["DefaultPicture"].ToString();
                    else
                        messageContent.MessagePicture = "";
                    messageContent.MessagePictureVisibility = "Visible";
                    messageContent.MessageText = item["Message"].ToString();

                    DateTime dateTime = Convert.ToDateTime(item["DateAndTime"].ToString());
                    messageContent.MessageTime = dateTime.ToString("HH:mm");

                    messageContent.Name = item["UserId"].ToString();
                    messageContent.Pic = null;
                    messageContent.PictureChanged = false;
                    messageContent.IdList = new List<string>();

                    jsonContainer.Messages.Add(messageContent);
                }
            }

            // add last dateTime when first time adding messages from DB 
            CurrentDate = tempDateTime;

        }
    }
}
