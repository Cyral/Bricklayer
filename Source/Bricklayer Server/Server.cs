#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using Bricklayer.API;
using Bricklayer.Common.Entities;
using Bricklayer.Common.Networking.Messages;
using Bricklayer.Common.World;
using Player = Bricklayer.Server.Entities.Player;
using Map = Bricklayer.Server.World.Map;
#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// The main server entry point
    /// </summary>
    public class Server : IPluginHost
    {
        #region Properties
        /// <summary>
        /// The server configuration settings, such as port, name, etc
        /// </summary>
        public static Settings Config { get; set; }
        /// <summary>
        /// The NetworkManager for handling recieving, sending, etc
        /// </summary>
        public static NetworkManager NetManager { get; set; }
        /// <summary>
        /// The PingListener for responding to query requests
        /// </summary>
        public static PingListener PingListener { get; set; }
        /// <summary>
        /// Handler for processing incoming messages
        /// </summary>
        public static MessageHandler MsgHandler { get; set; }
        /// <summary>
        /// The Map list for storing all open rooms
        /// </summary>
        public static List<Map> Maps { get; set; }
        /// <summary>
        /// Lookup of remote unique identifiers to login data
        /// </summary>
        public static Dictionary<long, LoginMessage> Logins = new Dictionary<long, LoginMessage>();
        /// <summary>
        /// List of currently loaded plugins
        /// </summary>
        public List<IPlugin> Plugins { get; set; }
        #endregion

        #region Fields
        #endregion

        /// <summary>
        /// Runs/Starts the server networking
        /// </summary>
        public void Run()
        {
            IO.LoadSettings(); //Load settings

            //Write a welcome message
            Program.Write("Bricklayer ", ConsoleColor.Yellow);
            Program.WriteLine("Server started on port " + Config.Port + " with " + Config.MaxPlayers + " max players.");
            Program.WriteBreak();

            LoadPlugins();

            MsgHandler = new MessageHandler();
            NetManager = new NetworkManager(); //Create Networkmanager to handle networking, then start the server
            NetManager.Start(Config.Port, Config.MaxPlayers);

            //Create a PingListener to handle query requests from clients (to serve decription, players online, etc)
            PingListener = new PingListener(Config.Port);
            PingListener.Start();

            //Create a default map
            Maps = new List<Map>();
            CreateMap("Main World", "A large world for anyone to play and\nbuild! [color:SkyBlue]--Join Now!--[/color]");

            Program.WriteLine("Waiting for new connections and updating world state...\n");
            MsgHandler.ProcessNetworkMessages(); //Process messages for the rest of eternity
        }

        /// <summary>
        /// Register a plugin by adding it to the plugin list and telling it to start
        /// </summary>
        /// <param name="plugin">Plugin to register</param>
        public void RegisterPlugin(IPlugin plugin)
        {
            Plugins.Add(plugin);
            plugin.Load();
        }

        /// <summary>
        /// Load plugins
        /// </summary>
        public void LoadPlugins()
        {
            Program.WriteLine("Loading plugins...", ConsoleColor.Green);

            //Load plugins
            Plugins = new List<IPlugin>();
            IO.LoadPlugins(this);

            Program.WriteLine(string.Format("{0} plugin{1} loaded.\n", Plugins.Count, Plugins.Count == 1 ? string.Empty : "s"), ConsoleColor.Green);
        }

        #region Utilities
        /// <summary>
        /// Creates a new map and adds it to the room list
        /// </summary>
        public static Map CreateMap(string name, string description)
        {
            Map map = new Map(name, description, 200, 100, Maps.Count) { Rating = 5 };
            Maps.Add(map);
            return map;
        }

        /// <summary>
        /// Finds a map from an ID
        /// </summary>
        /// <param name="ID">The ID of the map to find</param>
        public static Map MapFromID(int ID)
        {
            return Maps.First(m => m.ID == ID);
        }

        /// <summary>
        /// Finds a player from a remote unique identifier
        /// </summary>
        /// <param name="remoteUniqueIdentifier">The RMI to find</param>
        /// <param name="ignoreError">If a player is not found, should an error be thrown?</param>
        public static Player PlayerFromRUI(long remoteUniqueIdentifier, bool ignoreError = false)
        {
            Player found = null;
            foreach (Map map in Maps)
            {
                foreach (Player player in map.Players)
                {
                    if (player.RemoteUniqueIdentifier == remoteUniqueIdentifier)
                        found = player;
                }
            }
            if (found != null) return found;
            else if (ignoreError) return null;
            else throw new KeyNotFoundException("Could not find player from RemoteUniqueIdentifier: " + remoteUniqueIdentifier);
        }

        /// <summary>
        /// Finds an empty slot to use as a player's ID
        /// </summary>
        public static byte FindEmptyID(Map map)
        {
            for (int i = 0; i < Config.MaxPlayers; i++)
                if (!map.Players.Any(x => x.ID == i))
                    return (byte)i;
            Program.WriteLine("Could not find empty ID!", ConsoleColor.Red);
            return 0;
        }
        #endregion

    }
}