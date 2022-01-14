﻿using Domain;
using Domain.Converters;
using Domain.Data;
using Domain.Data.JsonContainers;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class ServerConfig
    {

        private static Socket ServerSocket { get; set; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint TcpEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 8080);

        public static ManualResetEvent ReadDone = new ManualResetEvent(false);

        public IUIViewModel _uIViewModel { get; set; }
        string PersonId = "";


        public ServerConfig(IUIViewModel uIViewModel)
        {
            _uIViewModel = uIViewModel;
            StartServer();
        }

        private void StartServer()
        {
            ServerSocket.Bind(TcpEndPoint);
            ServerSocket.Listen(10);

            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), ServerSocket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;


            PersonId = Guid.NewGuid().ToString();

            Person person = new Person();
            ChatSwitch chatSwitch = new ChatSwitch();

            BaseContainer jsonContainer = new BaseContainer()
            {
                Messages = new System.Collections.ObjectModel.ObservableCollection<MessageContent>(),
                Persons = new List<Person>(),
                Credential = new Credential()
            };

            MessageContainer jsonMessageContainer = new MessageContainer()
            {
                Switch = new ChatSwitch(),
                Message = new MessageContent()
            };

            MessageContent messageContent = new MessageContent();

            Credential credential = new Credential();

            CredentialConfirmation credentialConfirmation = new CredentialConfirmation();

            CredentialCheck credentialCheck = new CredentialCheck();

            Connect connect = new Connect(PersonId, chatSwitch, person,
                jsonContainer, jsonMessageContainer, messageContent,
                _uIViewModel, credential, credentialConfirmation, credentialCheck);

            connect.FirstTime = true;
            connect.FirstTimeConnect = true;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(connect.ReadCallback), state);
            ReadDone.WaitOne();
          
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                       

        }
            
    }
}
