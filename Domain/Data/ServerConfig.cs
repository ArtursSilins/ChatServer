using Domain;
using Domain.Data;
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
        int PersonId = 0;


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

            PersonId++;

            Person person = new Person();

            JsonContainer jsonContainer = new JsonContainer()
            {
                CurrentPersonId = new CurrentPersonId(),
                Messages = new System.Collections.ObjectModel.ObservableCollection<MessageContent>(),
                Persons = new List<Person>()
            };

            MessageContent messageContent = new MessageContent();

            Connect connect = new Connect(PersonId, person, jsonContainer, messageContent, _uIViewModel);
            connect.FirstTime = true;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(connect.ReadCallback), state);
            ReadDone.WaitOne();
          
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                       

        }
            
    }
}
