using ServerViewWPF.Common;
using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ServerViewWPF.ViewModel
{
    class ServerViewModel : INotifyPropertyChanged
    {
        // Holding the servername typed by user, and later if hostname or ip is valid, the whole WMI collection of host values.
        private Server server = new Server();

        // This list will hold all the servers that is placed inside our db
        private ObservableCollection<Server> serverList = new ObservableCollection<Server>();

        // Validation variable for checking if the server is inside the list or not.
        private bool serverInList = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private ICommand addHostCommand;

        // Default constructor
        public ServerViewModel()
        {
            AddHostCommand = new RelayCommand(AddNewHost, param => true);

            serverList = DalManager.Instance.GetAllServers();
        }

        public void AddNewHost(object obj)
        {
            // Collecting information on the typed hostname or IP with WMIManager.
            // Then we set the returned server object to ours.
            // Making sure that we have a value to search for.
            if(!string.IsNullOrEmpty(server.Name))
            {
                WMIManager wm = new WMIManager();
                Server hostValues = wm.WMICall(server.Name);

                // Handling if WMI failes to get values from host
                if(hostValues != null)
                {

                    // Lets check if the server is in the list already
                    // If not then we add the server to the list
                    foreach (Server s in serverList) {
                        if (hostValues.Name == s.Name) {
                            serverInList = true;
                        }
                    }

                    if (!serverInList)
                    {
                        server = hostValues;
                        server.Status = "Prod";
                        server = DalManager.Instance.CheckServer(server);
                        serverList.Add(server);
                    }                       
                }
            }
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

        public ObservableCollection<Server> ServerList
        {
            get
            {
                return serverList;
            }

            set
            {
                serverList = value;
                OnPropertyChanged("ServerList");
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
