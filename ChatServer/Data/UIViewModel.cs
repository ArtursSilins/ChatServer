using ChatServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Data
{
    public class UIViewModel : ViewModelBase, IUIViewModel
    {
        private string chatMessages;

        public string ChatMessages
        {
            get
            {
                return chatMessages;
            }
            set
            {
                chatMessages = value;
                OnPropertyChanged(nameof(ChatMessages));
            }
        }

        private string usersOnline;

        public string UsersOnline
        {
            get
            {
                return usersOnline;
            }
            set
            {
                usersOnline = value;
                OnPropertyChanged(nameof(UsersOnline));
            }
        }
    }
}
