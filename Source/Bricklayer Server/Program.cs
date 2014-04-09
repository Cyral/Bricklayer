using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Cyral.Extensions;
using Cyral.Extensions.Xna;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Bricklayer.Client.Entities;
using Bricklayer.Client.Networking.Messages;
using Bricklayer.Client.World;
using System.Collections.Generic;
using Bricklayer.Client.Networking;

namespace Bricklayer.Server
{
    public class Program
    {
        //Config
        public static Settings Config;
        
        //Networking
        public static NetworkManager NetManager;
        public static PingListener PingListener;
        public static List<Map> Maps;
        /// <summary>
        /// Lookup of remote unique identifiers to login data
        /// </summary>
        public static Dictionary<long, LoginMessage> Logins = new Dictionary<long, LoginMessage>();

        //Console Events
        static ConsoleEventDelegate consoleHandler; //Keeps it from getting garbage collected
        //PInvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static void Main(string[] args)
        {
            //Setup forms stuff, to possibly be used if an error occurs and a dialog needs to be opened
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            //Only show error screen in release
            #if DEBUG
            Run();
            #else
            try
            {
                Run();
            }
            catch (Exception e)
            {
                //Open all exceptions in an error dialog
                System.Windows.Forms.Application.Run(new Bricklayer.Client.ExceptionForm(e));
            }
            #endif
        }
        private static void Run()
        {
            Console.Title = "Bricklayer Server - " + AssemblyVersionName.GetVersion();
            //Setup console events
            consoleHandler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(consoleHandler, true);
            //Load settings
            IO.LoadSettings();
            //Create Networkmanager to handle networking, then start the server
            NetManager = new NetworkManager();
            NetManager.Start(Config.Port, Config.MaxPlayers);
            //Write a welcome message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Bricklayer ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Server started on port " + Config.Port + " with " + Config.MaxPlayers + " max players.");
            Console.WriteLine("Waiting for new connections and updating world state...\n");
            //Create a PingListener to handle query requests from clients (to serve decription, players online, etc)
            PingListener = new PingListener(Config.Port);
            PingListener.Start();

            //Used to store messages
            NetIncomingMessage inc;
            // Check time
            DateTime time = DateTime.Now;
            //Create a map
            Maps = new List<Map>();
            Maps.Add(new Map("Main World", "A Simple Map.\n'Nuff Said", 150, 75) { Rating = 2.5 });
            Maps.Add(new Map("Nub Crew Shift", "Wow. Much Innvovation. Very Copy\nSuch Troll. Ultra.", 150, 75) { Rating = 4.5 });
            Maps.Add(new Map("DerpyCraft", "Derp derp da da derp", 150, 75) { Rating = 1.5 });
            while (true)
            {
                if ((inc = NetManager.ReadMessage()) != null)
                {
                    Player sender = inc.SenderConnection == null ? null : PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier, true);
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                MessageTypes type = (MessageTypes)Enum.Parse(typeof(MessageTypes), inc.ReadByte().ToString());
                                switch (type)
                                {
                                    case MessageTypes.Login:
                                        {
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine("Incoming Login Request (" + inc.SenderConnection.RemoteUniqueIdentifier + " - " + inc.SenderEndPoint.Address + ")");
                                            Console.ForegroundColor = ConsoleColor.White;

                                            if (!Logins.ContainsKey(inc.SenderConnection.RemoteUniqueIdentifier))
                                            {
                                                //Parse login
                                                LoginMessage login = new LoginMessage(inc);
                                                Logins.Add(inc.SenderConnection.RemoteUniqueIdentifier, login);
                                                //Approve of the client 
                                                inc.SenderConnection.Approve();
                                            }
                                            else
                                            {
                                                inc.SenderConnection.Deny();
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        // Data type is all messages manually sent from client
                        case NetIncomingMessageType.Data:
                            {
                                ProcessDataMessage(inc);
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                            Console.Write(inc.SenderConnection.ToString() + " status changed: ");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine((NetConnectionStatus)inc.SenderConnection.Status);
                            Console.ForegroundColor = ConsoleColor.White;

                            //When a players connection is finalized
                            if (inc.SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                break;
                            }
                            //When a client disconnects
                            if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                if (sender != null)
                                {
                                    Console.WriteLine(sender.Username + " has left game (disconnected).");
                                    if (sender.Map.Players.Contains(sender))
                                    {
                                        //Remove player
                                        sender.Map.Players.Remove(sender);
                                        //Rebuild indexes
                                        for (int i = 0; i < sender.Map.Players.Count; i++)
                                            sender.Map.Players[i].Index = i;
                                        //Send to players
                                        NetManager.BroadcastMessage(sender.Map, new PlayerLeaveMessage(sender.ID));
                                        sender = null;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(inc.SenderConnection.RemoteUniqueIdentifier + " has left lobby (disconnected).");
                                }
                                Logins.Remove(inc.SenderConnection.RemoteUniqueIdentifier);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        default:
                            break;
                    }
                }
                //While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                System.Threading.Thread.Sleep(Config.Sleep);
            }
        }
        /// <summary>
        /// Handles all actions for recieving data messages (Such as movement, block placing, etc)
        /// </summary>
        /// <param name="inc">The Incoming Message</param>
        private static void ProcessDataMessage(NetIncomingMessage inc)
        {
            Player sender = PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier, true);
            Map map = null;
            if (sender != null)
               map = sender.Map;
            MessageTypes type = (MessageTypes)Enum.Parse(typeof(MessageTypes), inc.ReadByte().ToString());
            switch (type)
            {
                case MessageTypes.JoinRoom:
                    {
                        JoinRoomMessage msg = new JoinRoomMessage(inc);
                        int newMap = Maps.IndexOf(Maps.Where(m => m.Name == msg.Name).ToList()[0]);
                        LoginMessage login = Logins[inc.SenderConnection.RemoteUniqueIdentifier]; //Fetch stored login from dictionary
                        Maps[newMap].Players.Add(new Player(Maps[newMap], Maps[newMap].Spawn, login.Username, inc.SenderConnection.RemoteUniqueIdentifier, FindEmptyID(Maps[newMap])) { Tint = login.Color });
                        sender = PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier, true);
                        NetManager.SendMessage(new InitMessage(sender.Map), sender);
                        //Send message to player notifing he is connected and ready
                        NetManager.SendMessage(new PlayerJoinMessage(sender.Username, sender.ID, true, sender.Tint), sender);
                        Console.WriteLine(login.Username + " joined room: " + msg.Name);
                        //Send message to everyone notifying of new user
                        NetManager.BroadcastMessageButPlayer(new PlayerJoinMessage(sender.Username, sender.ID, false, sender.Tint), sender);
                        //Let new player know of all existing players and their states (Mode, Position, Smiley)
                        foreach (Player player in sender.Map.Players)
                        {
                            if (player.ID != sender.ID)
                            {
                                NetManager.SendMessage(new PlayerJoinMessage(player.Username, player.ID, false, player.Tint), sender);
                                NetManager.SendMessage(new PlayerStateMessage(player), sender);
                                if (player.Mode != PlayerMode.Normal)
                                    NetManager.SendMessage(new PlayerModeMessage(player), sender);
                                if (player.Smiley != SmileyType.Default)
                                    NetManager.SendMessage(new PlayerSmileyMessage(player, player.Smiley), sender);
                            }
                        }
                        Console.WriteLine("\tUsername: " + sender.Username);
                        Console.WriteLine("\tRemote Unique Identifier: " + sender.RUI);
                        Console.WriteLine("\tNetwork ID: " + sender.ID);
                        Console.WriteLine("\tPlayer Index: " + sender.Index);
                        Console.WriteLine("\tIP: " + inc.SenderEndPoint.Address);
                        break;
                    }
                case MessageTypes.CreateRoom:
                    {
                        CreateRoomMessage msg = new CreateRoomMessage(inc);
                        Map newMap = new Map(msg.Name, msg.Description, 200, 100);
                        Maps.Add(newMap);
                        LoginMessage login = Logins[inc.SenderConnection.RemoteUniqueIdentifier]; //Fetch stored login from dictionary
                        newMap.Players.Add(new Player(newMap, newMap.Spawn, login.Username, inc.SenderConnection.RemoteUniqueIdentifier, FindEmptyID(newMap)) { Tint = login.Color });
                        sender = PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier, true);
                        //Send message to player notifing he is connected and ready
                        NetManager.SendMessage(new PlayerJoinMessage(sender.Username, sender.ID, true, sender.Tint), sender);
                        NetManager.SendMessage(new InitMessage(sender.Map), sender);
                        Console.WriteLine(login.Username + " created new room: " + msg.Name);
                        break;
                    }
                case MessageTypes.Request:
                    {
                        MessageTypes request = new RequestMessage(inc).RequestType;
                        if (request == MessageTypes.Lobby)
                        {
                            //Send them the lobby data
                            List<LobbySaveData> rooms = new List<LobbySaveData>();
                            foreach (Map m in Maps)
                            {
                                rooms.Add(LobbySaveData.FromMap(m));
                            }

                            LobbyMessage msg = new LobbyMessage(Config.Name, Config.Decription, Config.Intro, NetworkManager.Server.ConnectionsCount, rooms);
                            NetManager.SendMessage(msg, inc.SenderConnection);
                        }
                        break;
                    }
                case MessageTypes.PlayerLeave:
                    {
                        PlayerLeaveMessage user = new PlayerLeaveMessage(inc);
                        user.ID = sender.ID;
                        Console.WriteLine(map.PlayerFromID(user.ID).Username + " has left.");
                        //Remove player
                        map.Players.Remove(sender);
                        //Rebuild indexes
                        for (int i = 0; i < map.Players.Count; i++)
                            map.Players[i].Index = i;
                        //Send to players
                        NetManager.BroadcastMessage(sender.Map,new PlayerLeaveMessage(sender.ID));
                        sender = null;
                        break;
                    }
                case MessageTypes.PlayerStatus:
                    {
                        PlayerStateMessage state = new PlayerStateMessage(inc);
                        //Console.WriteLine(state.ID + ":" + sender.ID + "  " + state.Position.X + "," + state.Position.Y);
                        state.ID = sender.ID;
                        //Clamp position in bounds
                        state.Position = new Point((int)MathHelper.Clamp(state.Position.X, Tile.Width, (map.Width * Tile.Width) - (Tile.Width * 2)), (int)MathHelper.Clamp(state.Position.Y, Tile.Height, (map.Height * Tile.Height) - (Tile.Height * 2)));
                        sender.SimulationState.Position = state.Position.ToVector2();
                        sender.SimulationState.Velocity = state.Velocity.ToVector2();
                        sender.SimulationState.Movement = state.Movement.ToVector2();
                        sender.IsJumping = state.IsJumping;

                        NetManager.BroadcastMessageButPlayer(state, sender);
                        break;
                    }
                case MessageTypes.Block:
                    {
                        BlockMessage state = new BlockMessage(inc);
                        BlockType block = BlockType.FromID(state.BlockID);
                        //Verify Block (Make sure it is in bounds and it has changed, no use sending otherwise)
                        //TODO: Punish players spamming invalid data (Because official client should never send it)
                        if (map.InBounds(state.X, state.Y, state.Z) && map.Tiles[state.X, state.Y, state.Z].Block.ID != block.ID)
                        {
                            map.Tiles[state.X, state.Y, state.Z].Block = block;
                            NetManager.BroadcastMessage(sender.Map, state);
                        }
                        break;
                    }
                case MessageTypes.Chat:
                    {
                        ChatMessage chat = new ChatMessage(inc);
                        chat.ID = sender.ID;
                        //Verify Chat
                        if (chat.Message.Length > ChatMessage.MaxLength)
                            chat.Message = chat.Message.Truncate(ChatMessage.MaxLength);
                        NetManager.BroadcastMessageButPlayer(chat, sender);
                        break;
                    }
                case MessageTypes.PlayerSmiley:
                    {
                        PlayerSmileyMessage smiley = new PlayerSmileyMessage(inc);
                        smiley.ID = sender.ID;
                        sender.Smiley = smiley.Smiley;
                        NetManager.BroadcastMessageButPlayer(smiley, sender);
                        break;
                    }
                case MessageTypes.PlayerMode:
                    {
                        PlayerModeMessage mode = new PlayerModeMessage(inc);
                        mode.ID = sender.ID;
                        sender.Mode = mode.Mode;
                        NetManager.BroadcastMessageButPlayer(mode, sender);
                        break;
                    }
            }
        }
        #region Utilities
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
                    if (player.RUI == remoteUniqueIdentifier)
                        found = player;
                }
            }
            if (found != null)
                return found;
            else if (ignoreError)
                return null;
            else
                throw new KeyNotFoundException("Could not find player from RemoteUniqueIdentifier: " + remoteUniqueIdentifier);
        }
        /// <summary>
        /// Finds an empty slot to use as a player's ID
        /// </summary>
        public static byte FindEmptyID(Map map)
        {
            for (int i = 0; i < Config.MaxPlayers; i++)
                if (!map.Players.Any(x => x.ID == i))
                    return (byte)i;
            Console.WriteLine("Could not find empty ID!");
            return 0;
        }
        /// <summary>
        /// Used to handle events from the console
        /// </summary>
        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                //Console closing
                NetManager.Disconnect("Server Closed by Operator");
            }
            return false;
        }
        #endregion
    }
}