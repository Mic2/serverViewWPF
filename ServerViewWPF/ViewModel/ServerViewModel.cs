using ServerViewWPF.Common;
using ServerViewWPF.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ServerViewWPF.ViewModel
{
    class ServerViewModel : INotifyPropertyChanged
    {
        // Holding the servername typed by user, and later if hostname or ip is valid, the whole WMI collection of host values.
        private Server server = new Server();

        // This list will hold all the servers that is placed inside our db
        private static ObservableCollection<Server> serverList = new ObservableCollection<Server>();

        // Validation variable for checking if the server is inside the list or not.
        private bool serverInList = false;

        Thread updateServerListThread = new Thread(startServerListUpdate);
   
        public event PropertyChangedEventHandler PropertyChanged;
        private ICommand addHostCommand;

        // Default constructor
        public ServerViewModel()
        {
            serverList = DalManager.Instance.GetAllServers();

            AddHostCommand = new RelayCommand(AddNewHost, param => true);

            // Starting the Thread that listens for new info in DB
            updateServerListThread.Start();
            
        }

        public static void startServerListUpdate()
        {
            while (true)
            {

                ObservableCollection<Server> serversFromDb = DalManager.Instance.GetAllServers();
                CompareServerLists cs = new CompareServerLists();
                cs.GatherServerNames(serverList, serversFromDb);

                foreach (Server server in cs.ServersToAdd)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        // Use the Dispatcher to push this to the UI thread
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            serverList.Add(server);
                        }));
                    });
                }

                foreach (Server server in cs.ServersToRemove)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        // Use the Dispatcher to push this to the UI thread
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            serverList.Remove(server);
                        }));
                    });  
                }

                Debug.WriteLine("Thread is running");
                Thread.Sleep(10000);
            }
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
                    foreach (Server s in serverList) {
                        // We need to trim since s.Name is equal 250 length "DONT KNOW WHY"
                        if (hostValues.Name == s.Name.Trim()) {
                            serverInList = true;
                        }
                    }

                    // If the server is not in the list, we create it in DB and adds it to the list
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
