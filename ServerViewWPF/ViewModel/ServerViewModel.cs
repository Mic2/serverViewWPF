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

        Thread updateServerListThread = new Thread(startServerListUpdate);
   
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand addHostCommand;
        private ICommand removeHostsCommand;
        private ICommand updateHostsCommand;

        // Default constructor
        public ServerViewModel()
        {
            serverList = DalManager.Instance.GetAllServers();
            AddHostCommand = new RelayCommand(AddNewHost, param => true);
            RemoveHostsCommand = new RelayCommand(RemoveHosts, param => true);
            UpdateHostsCommand = new RelayCommand(UpdateHosts, param => true);

            updateServerListThread.IsBackground = true;
            // Starting the Thread that listens for new info in DB
            updateServerListThread.Start();
            
        }

        public void UpdateHosts(object selectedItems)
        {
            Debug.WriteLine("Update triggered");
            var itemsFromList = (System.Collections.IList)selectedItems;
            ObservableCollection<Server> serversToUpdate = new ObservableCollection<Server>();
            WMIManager wm = new WMIManager();

            foreach (Server server in itemsFromList)
            {
                serversToUpdate.Add(server);
            }

            foreach (Server server in serversToUpdate)
            {
                
                Server hostValues = wm.WMICall(server.Name.Trim());

                if (hostValues != null)
                {
                    ServerList.Remove(server);
                    DalManager.Instance.DeleteServer(server);
                    // Validation variable for checking if the server is inside the list or not.
                    bool serverInList = false;

                    // Lets check if the server is in the list already
                    foreach (Server s in serverList)
                    {
                        // We need to trim since s.Name is equal 250 length "DONT KNOW WHY"
                        if (hostValues.Name == s.Name.Trim())
                        {
                            serverInList = true;
                        }
                    }

                    if(!serverInList)
                    {
                        Debug.WriteLine("We are in here");
                        hostValues.Status = "Prod";
                        Server updatedServer = DalManager.Instance.CheckServer(hostValues);
                        serverList.Add(updatedServer);
                    }  
                }
            }
        }

        public void RemoveHosts(object selectedItems)
        {
            var itemsFromList = (System.Collections.IList)selectedItems;
            ObservableCollection<Server> serversToRemove = new ObservableCollection<Server>();

            foreach (Server server in itemsFromList)
            {
                serversToRemove.Add(server);
            }

            foreach (Server server in serversToRemove)
            {
                ServerList.Remove(server);
                DalManager.Instance.DeleteServer(server);
            }
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
                // Validation variable for checking if the server is inside the list or not.
                bool serverInList = false;

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

        public ICommand RemoveHostsCommand
        {
            get
            {
                return removeHostsCommand;
            }

            set
            {
                removeHostsCommand = value;
            }
        }

        public ICommand UpdateHostsCommand
        {
            get
            {
                return updateHostsCommand;
            }

            set
            {
                updateHostsCommand = value;
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
