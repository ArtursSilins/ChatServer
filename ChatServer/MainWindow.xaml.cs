using Domain.Data;
using System.Windows;

namespace ChatServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new UIViewModel();

            Domain.DBOperations.DBAdd.Add = new Repository.Add();
            Domain.DBOperations.DBGet.Get = new Repository.Get();
            Domain.DBOperations.DBCheck.Check = new Repository.Check();
        }
      
    }
}
