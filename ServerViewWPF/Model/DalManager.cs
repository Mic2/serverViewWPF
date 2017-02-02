using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace App1.ViewModel
{
    class DalManager
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

        //NOTE: change '(localdb)\Test_env' to the server and db on remote
        private string dbCon = @"Data Source=(localdb)\Test_env;Initial Catalog=Test_db;Integrated Security=True";
        private SqlConnection sqlCon = null;

        //retive data and send it to the requester
        public string GetName()
        {
            throw new NotImplementedException();
            return null;
        }

        public IPAddress GetIP()
        {
            throw new NotImplementedException();
            return null;
        }

        public string GetMAC()
        {
            throw new NotImplementedException();
            return null;
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
            return null;
        }
    }
}
