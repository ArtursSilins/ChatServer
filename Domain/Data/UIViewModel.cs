using Domain.Interfaces;
using Domain.Data;

namespace Domain.Data
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
        public UIViewModel()
        {
            ServerConfig serverConfig = new ServerConfig(this);
        }
    }
}
