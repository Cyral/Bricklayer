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
        /// The name of the server
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The "Message of the day/Description", to be shown in the server list
        /// </summary>
        public string Decription { get; set; }
        /// <summary>
        /// The extended MOTD, showing possibly news, stats, etc.
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// The port the server should run on
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The maximum allowed players for the server (not per room)
        /// </summary>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// Represents the rate the server should sleep between updates
        /// Higher values cause less CPU usage, but more lag in networking
        /// </summary>
        public int Sleep { get; set; }

        public static Settings GetDefaultSettings()
        {
            return new Settings()
            {
                Sleep = 1,
                Port = Common.GlobalSettings.DefaultPort,
                MaxPlayers = 8,
                Name = "Bricklayer Server",
                Decription = "A Bricklayer Server running on the default configuration.\nPlease edit your Message Of The Day in the config file!",
                Intro = "Welcome to $Name!\nWe currently have $Online player(s) in $Rooms room(s).\n\nServer News:\n-\n-\n-\n\nServer Rules:\n-\n-\n-\n\n\n\n\n...",
            };
        }
    }
}
