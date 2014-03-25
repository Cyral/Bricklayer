﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BricklayerClient.Networking
{
    public class ServerPingData
    {
        public int Online;
        public int MaxOnline;
        public int Ping;
        public string MOTD;
        public bool Error;
    }
}