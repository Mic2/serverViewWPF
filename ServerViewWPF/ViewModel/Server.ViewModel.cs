using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerViewWPF.ViewModel
{
    class ServerViewModel : INotifyPropertyChanged
    {
        private Server server;

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerViewModel()
        {
            // Initiating server object (Dont really know if this shoul be done in constructor?)
            server = new Server();
            server.Name = "Jonathan";
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public Server Server
        {
            get
            {
                // So ive tried this... and that does not work :D 
                // It will throw an error. so we can only use the OnPropertyChanged at "set"
                //OnPropertyChanged("Server");
                return server;
            }

            set
            {
                server = value;

                // Notifiyng to the view that we have a change (this specifik case we got it from our constructor)
                OnPropertyChanged("Server");
            }
        }
    }
}
