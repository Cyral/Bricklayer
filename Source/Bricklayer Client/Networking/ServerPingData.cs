using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client.Networking
{
    /// <summary>
    /// Represents data recieved when pinging a server for players, description, etc
    /// </summary>
    public class ServerPingData
    {
        public int Online { get; set; }
        public int MaxOnline { get; set; }
        public int Ping { get; set; }
        public string Description { get; set; }
        public bool Error { get; set; }
    }
}
