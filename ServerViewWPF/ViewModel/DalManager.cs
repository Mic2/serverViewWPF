using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ServerViewWPF.ViewModel
{
    public class DalManager
    {
        //singleton
        private static DalManager _instance = null;
        private DalManager()
        {

        }
        public static DalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DalManager();
                }
                return _instance;
            }
        }
        private SqlConnection ConnectDB()
        {
            //NOTE: change here if you change the server or db on remote
            string connectionString = "Server = tcp:serverview.database.windows.net,1433;" +
                                      "Initial Catalog = ServerViewWPF;" +
                                      "Persist Security Info = False;" +
                                      "User ID = serverView;" +
                                      "Password = P@ssw0rd1;" +
                                      "MultipleActiveResultSets = False;" +
                                      "Encrypt = True;" +
                                      "TrustServerCertificate = False;" +
                                      "Connection Timeout = 30";
            SqlConnection myConnection = new SqlConnection(connectionString);
            return myConnection;
        }
        //retive data and send it to the requester
        public Server GetServer(string hostname)
        {
            Server server = new Server();
            SqlConnection myConnection = ConnectDB();
            // Provide the query string with a parameter placeholder.
            try
            {
                string queryString = "SELECT * from dbo.server WHERE Name = '" + hostname + "'";
                SqlCommand command = new SqlCommand(queryString, myConnection);
                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    server.Name = reader[0].ToString();
                    server.Status = reader[1].ToString();
                    server.Ram = Convert.ToInt64(reader[2]);
                    server.OsVer = reader[3].ToString();
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                string queryString = "SELECT * from dbo.netcard WHERE SFK_ID = '" + hostname + "'";
                SqlCommand command = new SqlCommand(queryString, myConnection);
                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    NetWorkCard Card = new NetWorkCard();
                    Card.IpAddress = reader[1].ToString();
                    Card.MacAddress = reader[2].ToString();
                    server.NetWorkCard.Add(Card);
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                string queryString = "SELECT * from dbo.Hdd WHERE SFK_ID = '" + hostname + "'";
                SqlCommand command = new SqlCommand(queryString, myConnection);
                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Harddisk Disk = new Harddisk();
                    Disk.DriveLetter = reader[1].ToString();
                    Disk.MbSize = Convert.ToInt64(reader[2]);
                    server.Hdd.Add(Disk);
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return server;
        }
        public void SetServer(Server server)
        {
            SqlConnection myConnection = ConnectDB();
            myConnection.Close();
        }
    }
}
