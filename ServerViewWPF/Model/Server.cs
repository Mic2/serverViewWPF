﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ServerViewWPF.Model
{
    public class Server
    {
        string name;
        string mac;
        string status;
        long ram;
        string osVer;
        List<NetWorkCard> netWorkCard;
        List<Harddisk> hdd = new List<Harddisk>();
        public Server()
        {
            netWorkCard = new List<NetWorkCard>();
            
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Mac
        {
            get { return mac; }
            set { mac = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public long Ram
        {
            get { return ram; }
            set { ram = value; }
        }
        public string OsVer
        {
            get { return osVer; }
            set { osVer = value; }
        }
        public List<NetWorkCard> NetWorkCard
        {
            get { return netWorkCard; }
            set { netWorkCard = value; }
        }
        public List<Harddisk> Hdd
        {
            get { return hdd; }
            set { hdd = value; }
        }
    }
}
