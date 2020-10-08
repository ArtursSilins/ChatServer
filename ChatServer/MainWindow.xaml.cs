using ChatServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint TcpEndPoint = new IPEndPoint(IPAddress.Any, 8080);

        public static byte[] buffer = new byte[1024];

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        int PersonId = 0;

        public UIViewModel uIViewModel { get; set; } = new UIViewModel();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = uIViewModel;

            StartServer();
        }
        private void StartServer()
        {
            ServerSocket.Bind(TcpEndPoint);
            ServerSocket.Listen(1);
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = ServerSocket.EndAccept(ar);

            PersonId++;

            Person person = new Person();

            JsonContainer jsonContainer = new JsonContainer()
            {
                CurrentPersonId = new CurrentPersonId(),
                Messages = new System.Collections.ObjectModel.ObservableCollection<MessageContent>(),
                Persons = new List<Person>()
            };

            MessageContent messageContent = new MessageContent();

            Connect connect = new Connect(PersonId, person, jsonContainer, messageContent, uIViewModel);

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(connect.ReceiveCallback), socket);

            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
