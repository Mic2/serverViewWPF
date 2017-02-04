using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Data;
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

            // Insert data to Server
            SqlCommand Servercmd = new SqlCommand("INSERT INTO dbo.server (Name, Status, Ram, OS) VALUES (@Name, @Status, @Ram, @OS)");
            Servercmd.CommandType = CommandType.Text;
            Servercmd.Connection = myConnection;
            Servercmd.Parameters.AddWithValue("@Name", server.Name);
            Servercmd.Parameters.AddWithValue("@Status", server.Status);
            Servercmd.Parameters.AddWithValue("@Ram", server.Ram);
            Servercmd.Parameters.AddWithValue("@OS", server.OsVer);
            myConnection.Open(); ;
            Servercmd.ExecuteNonQuery();
            myConnection.Close();
            // Insert data to Harddisk
            foreach (Harddisk Disk in server.Hdd)
            {
                SqlCommand Diskcmd = new SqlCommand("INSERT INTO dbo.Hdd (DriveLetter, Size_MB, SFK_ID) VALUES (@DriveLetter, @Size_MB, @SFK_ID)");
                Diskcmd.CommandType = CommandType.Text;
                Diskcmd.Connection = myConnection;
                Diskcmd.Parameters.AddWithValue("@DriveLetter", Disk.DriveLetter);
                Diskcmd.Parameters.AddWithValue("@Size_MB", Disk.MbSize);
                Diskcmd.Parameters.AddWithValue("@SFK_ID", server.Name);
                myConnection.Open(); ;
                Diskcmd.ExecuteNonQuery();
                myConnection.Close();
            }
            // Insert Data to NetworkCard
            foreach (NetWorkCard Card in server.NetWorkCard)
            {
                SqlCommand Cardcmd = new SqlCommand("INSERT INTO dbo.Netcard (IP, MAC, SFK_ID) VALUES (@IP, @MAC, @SFK_ID)");
                Cardcmd.CommandType = CommandType.Text;
                Cardcmd.Connection = myConnection;
                Cardcmd.Parameters.AddWithValue("@IP", Card.IpAddress);
                Cardcmd.Parameters.AddWithValue("@MAC", Card.MacAddress);
                Cardcmd.Parameters.AddWithValue("@SFK_ID", server.Name);
                myConnection.Open(); ;
                Cardcmd.ExecuteNonQuery();
                myConnection.Close();
            }
        }
    }
}
