using ServerViewWPF.Common;
using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ServerViewWPF.ViewModel
{
    class ServerViewModel : INotifyPropertyChanged
    {
        private Server server = new Server();

        public event PropertyChangedEventHandler PropertyChanged;
        private ICommand addHostCommand;

        // Default constructor
        public ServerViewModel()
        {
            AddHostCommand = new RelayCommand(AddNewHost, param => true);
        }

        public void AddNewHost(object obj)
        {
            WMIManager wm = new WMIManager();
            Server hostValues = wm.WMICall(server.Name);
            server = hostValues;
            Debug.WriteLine("Button pressed - Name of server is: " + server.Name);
            Debug.WriteLine(server.OsVer);
        }

        public Server Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
                OnPropertyChanged("Server");
            }
        }

        public ICommand AddHostCommand
        {
            get
            {
                return addHostCommand;
            }

            set
            {
                addHostCommand = value;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
