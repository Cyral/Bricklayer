using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Server
{
    /// <summary>
    /// Contains settings to be serialized into JSON
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The port the server should run on
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The maximum allowed players for the server (not per room)
        /// </summary>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// The "Message of the day", to be shown in the server list
        /// </summary>
        public string MOTD { get; set; }
        /// <summary>
        /// Represents the rate the server should sleep between updates
        /// Higher values cause less CPU useage, but more lag networking
        /// </summary>
        public int Sleep { get; set; }

        public static Settings GetDefaultSettings()
        {
            return new Settings()
            {
                Sleep = 1,
                Port = 14242,
                MaxPlayers = 8,
                MOTD = "A Bricklayer Server running on the default configuration.\nPlease edit your Message Of The Day in the config file!"
            };
        }
    }
}
