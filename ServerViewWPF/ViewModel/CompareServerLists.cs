using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerViewWPF.Model
{
    class CompareServerLists
    {
        ObservableCollection<Server> serversToAdd = new ObservableCollection<Server>();
        ObservableCollection<Server> serversToRemove = new ObservableCollection<Server>();

        public void GatherServerNames(ObservableCollection<Server> serverList, ObservableCollection<Server> serversFromDb)
        {
            List<string> tmpServerListNames = new List<string>();
            List<string> tmpServerFromDbList = new List<string>();

            // Just getting all names of the observablelist used i UI,
            // And store it in tmp list for compare later.
            foreach (Server server in serverList)
            {
                tmpServerListNames.Add(server.Name);
            }

            foreach (Server server in serversFromDb)
            {
                tmpServerFromDbList.Add(server.Name);
            }

            IEnumerable<string> itemsToRemove = tmpServerListNames.Except(tmpServerFromDbList);
            IEnumerable<string> itemsToAdd = tmpServerFromDbList.Except(tmpServerListNames);

            foreach (string item in itemsToAdd)
            {
                foreach (Server server in serversFromDb)
                {
                    if(server.Name == item)
                    {
                        ServersToAdd.Add(server);
                    }
                }
            }

            foreach (string item in itemsToRemove)
            {
                foreach (Server server in serverList)
                {
                    if (server.Name == item)
                    {
                        ServersToRemove.Add(server);
                    }
                }
            }
        }

        public ObservableCollection<Server> ServersToAdd
        {
            get
            {
                return serversToAdd;
            }

            set
            {
                serversToAdd = value;
            }
        }

        public ObservableCollection<Server> ServersToRemove
        {
            get
            {
                return serversToRemove;
            }

            set
            {
                serversToRemove = value;
            }
        }

    }
}
