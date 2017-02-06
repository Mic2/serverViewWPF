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
            //NOTE: MultipleActiveResultSets MUST be set to 'True'
            string connectionString = "Server = tcp:serverview.database.windows.net,1433;" +
                                        "Initial Catalog = ServerViewWPF;" +
                                        "Persist Security Info = False;" +
                                        "User ID = serverView;" +
                                        "Password = P@ssw0rd1;" +
                                        "MultipleActiveResultSets = True;" +
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

        //checks if the requested server exists in the db
        //calls getServer if it does
        //calls setSever if it does not
        public Server CheckServer(Server server)
        {
            //seleect the first matching resault
            string queryString = "SELECT COUNT(1) FROM dbo.Server WHERE Name = '" + server.Name + "'";
            int serverCount = 0;

            SqlConnection sqlCon = ConnectDB();
            SqlCommand command = new SqlCommand(queryString, sqlCon);
            Server s = new Server();

            try
            {
                sqlCon.Open();
                serverCount = (int)command.ExecuteScalar(); //returns amount of resaults

                //did we find a server with the same name?
                if (serverCount > 0)
                {
                    //grab info about the server
                    s = GetServer(server.Name);
                }
                else
                {
                    //add new info about the server
                    SetServer(server);
                    s = server;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //close SQL connection no matter what happens
            finally
            {
                sqlCon.Close();
            }

            return s;
        }

        //returns a list of all servers
        public List<Server> GetAllServers()
        {
            string queryString_getServes = "SELECT * FROM dbo.Server";

            SqlConnection sqlCon = ConnectDB();
            SqlCommand command_getServes = new SqlCommand(queryString_getServes, sqlCon);
            List<Server> servers = new List<Server>();

            sqlCon.Open();

            try
            {
                //get servers
                SqlDataReader reader = command_getServes.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("Reading " + reader[0].ToString().Trim());
                    Server server = new Server();

                    server.Name = reader[0].ToString();
                    server.Status = reader[1].ToString();
                    server.Ram = Convert.ToInt64(reader[2]);
                    server.OsVer = reader[3].ToString();

                    servers.Add(server);
                }


                //get networks cards
                for (int i = 0; i < servers.Count; i++)
                {
                    //find all netcards related to the current server
                    string queryString_getNetCards = "SELECT * from dbo.netcard WHERE SFK_ID = '" + servers[i].Name + "'";
                    SqlCommand command_getNetCards = new SqlCommand(queryString_getNetCards, sqlCon);
                    reader = command_getNetCards.ExecuteReader();

                    while (reader.Read())
                    {
                        //create a new netcard and add it to the server
                        NetWorkCard Card = new NetWorkCard();
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Reading NetCard ID " + reader[0].ToString().Trim() + " for " + reader[3].ToString().Trim() + ". Current Server: " + servers[i].Name.ToString().Trim());
                        Console.ResetColor();

                        Card.IpAddress = reader[1].ToString();
                        Card.MacAddress = reader[2].ToString();
                        servers[i].NetWorkCard.Add(Card);

                        Console.WriteLine("Server " + servers[i].Name.ToString().Trim() + " now has " + servers[i].NetWorkCard.Count.ToString().Trim() + " netcards.");
                        foreach (NetWorkCard n in servers[i].NetWorkCard)
                        {
                            Console.WriteLine("IP: " + n.IpAddress.ToString().Trim() + " MAC:" + n.MacAddress.ToString().Trim());
                        }
                    }
                }


                //get hdd's
                for (int i = 0; i < servers.Count; i++)
                {
                    //find all hdd's related to the current server
                    string queryString_getHDDs = "SELECT * from dbo.hdd WHERE SFK_ID = '" + servers[i].Name + "'";
                    SqlCommand command_getHDDs = new SqlCommand(queryString_getHDDs, sqlCon);
                    reader = command_getHDDs.ExecuteReader();

                    while (reader.Read())
                    {
                        //create a new hdd and add it to the server
                        Harddisk hdd = new Harddisk();
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Reading HDD ID " + reader[0].ToString().Trim() + " for " + reader[3].ToString().Trim() + ". Current Server: " + servers[i].Name.ToString().Trim());
                        Console.ResetColor();

                        hdd.DriveLetter = reader[1].ToString();
                        hdd.MbSize = (int)reader[2];
                        servers[i].Hdd.Add(hdd);

                        Console.WriteLine("Server " + servers[i].Name.ToString().Trim() + " now has " + servers[i].Hdd.Count.ToString().Trim() + " Hdd's.");
                        foreach (Harddisk h in servers[i].Hdd)
                        {
                            Console.WriteLine("Drive letter: " + h.DriveLetter.ToString().Trim() + " Size: " + h.MbSize.ToString().Trim());
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                sqlCon.Close();
            }

            return servers;
        }
    }
}