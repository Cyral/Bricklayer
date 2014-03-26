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

namespace Bricklayer.Server
{
    public class Program
    {
        //Config
        private const int Sleep = 1;
        
        //Networking
        public static NetworkManager NetManager;
        public static PingListener PingListener;
        public static Map Map;
        public static string Motd;
        public static int Port, MaxConnections;

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
                System.Windows.Forms.Application.Run(new BricklayerClient.ExceptionForm(e));
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
            NetManager.Start(Port, MaxConnections);
            //Write a welcome message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Bricklayer ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Server started on port " + Port + " with " + MaxConnections + " max players.");
            Console.WriteLine("Waiting for new connections and updating world state...\n");
            //Create a PingListener to handle query requests from clients (to serve motd, players online, etc)
            PingListener = new PingListener(Port);
            PingListener.Start();

            //Used to store messages
            NetIncomingMessage inc;
            // Check time
            DateTime time = DateTime.Now;
            //Create a map
            Map = new Map(100, 50);

            while (true)
            {
                if ((inc = NetManager.ReadMessage()) != null)
                {
                    Player sender = inc.SenderConnection == null ? null : Map.PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier, true);
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

                                            //Parse login
                                            LoginMessage login = new LoginMessage(inc);
                                            //Approve of the client 
                                            inc.SenderConnection.Approve();
                                            Map.Players.Add(new Player(Map, new Vector2(100, 100), login.Username, inc.SenderConnection.RemoteUniqueIdentifier, FindEmptyID()));
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
                                //Send message to player notifing he is connected and ready
                                NetManager.SendMessage(new PlayerJoinMessage(sender.Username, sender.ID, true), sender);
                                //Send message to everyone notifying of new user
                                NetManager.BroadcastMessageButPlayer(new PlayerJoinMessage(sender.Username, sender.ID, false), sender);
                                //Let new player know of all existing players and their states (Mode, Position, Smiley)
                                foreach (Player player in Map.Players)
                                {
                                    if (player.ID != sender.ID)
                                    {
                                        NetManager.SendMessage(new PlayerJoinMessage(player.Username, player.ID, false), sender);
                                        NetManager.SendMessage(new PlayerStateMessage(player), sender);
                                        if (player.Mode != PlayerMode.Normal)
                                            NetManager.SendMessage(new PlayerModeMessage(player), sender);
                                        if (player.Smiley != SmileyType.Default)
                                            NetManager.SendMessage(new PlayerSmileyMessage(player, player.Smiley), sender);
                                    }
                                }
                                NetManager.SendMessage(new InitMessage(Map), sender);
                                Console.WriteLine("\tUsername: " + sender.Username);
                                Console.WriteLine("\tRemote Unique Identifier: " + sender.RUI);
                                Console.WriteLine("\tNetwork ID: " + sender.ID);
                                Console.WriteLine("\tPlayer Index: " + sender.Index);
                                Console.WriteLine("\tIP: " + inc.SenderEndPoint.Address);
                                break;
                            }
                            //When a client disconnects
                            if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(sender.Username + " has left (disconnected).");
                                Console.ForegroundColor = ConsoleColor.White;
                                if (Map.Players.Contains(sender))
                                {
                                    //Remove player
                                    Map.Players.Remove(sender);
                                    //Rebuild indexes
                                    for (int i = 0; i < Map.Players.Count; i++)
                                        Map.Players[i].Index = i;
                                    //Send to players
                                    NetManager.BroadcastMessage(new PlayerLeaveMessage(sender.ID));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                //While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                System.Threading.Thread.Sleep(Sleep);
            }
        }
        /// <summary>
        /// Handles all actions for recieving data messages (Such as movement, block placing, etc)
        /// </summary>
        /// <param name="inc">The Incoming Message</param>
        private static void ProcessDataMessage(NetIncomingMessage inc)
        {
            Player sender = Map.PlayerFromRUI(inc.SenderConnection.RemoteUniqueIdentifier);
            MessageTypes type = (MessageTypes)Enum.Parse(typeof(MessageTypes), inc.ReadByte().ToString());
            switch (type)
            {
                case MessageTypes.PlayerLeave:
                    {
                        PlayerLeaveMessage user = new PlayerLeaveMessage(inc);
                        user.ID = sender.ID;
                        //Remove player
                        Map.Players.Remove(Map.PlayerFromID(user.ID));
                        //Rebuild indexes
                        for (int i = 0; i < Map.Players.Count; i++)
                            Map.Players[i].Index = i;
                        //Send to players
                        NetManager.BroadcastMessage(user);
                        Console.WriteLine(Map.PlayerFromID(user.ID).Username + " has left.");
                        break;
                    }
                case MessageTypes.PlayerStatus:
                    {
                        PlayerStateMessage state = new PlayerStateMessage(inc);
                        //Console.WriteLine(state.ID + ":" + sender.ID + "  " + state.Position.X + "," + state.Position.Y);
                        state.ID = sender.ID;
                        //Clamp position in bounds
                        state.Position = new Point((int)MathHelper.Clamp(state.Position.X, Tile.WIDTH, (Map.Width * Tile.WIDTH) - (Tile.WIDTH * 2)), (int)MathHelper.Clamp(state.Position.Y, Tile.HEIGHT, (Map.Height * Tile.HEIGHT) - (Tile.HEIGHT * 2)));
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
                        //Verify Block
                        if (Map.InBounds(state.X, state.Y))
                        {
                            Map.Tiles[state.X, state.Y].Foreground = BlockType.FromID(state.BlockID);
                            NetManager.BroadcastMessage(state);
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
        /// Finds an empty slot to use as a player's ID
        /// </summary>
        public static byte FindEmptyID()
        {
            for (int i = 0; i < MaxConnections; i++)
                if (!Map.Players.Any(x => x.ID == i))
                    return (byte)i;
            Console.WriteLine("Could not find empty ID!");
            return 0;
        }
        /// <summary>
        /// Used to handle events from the console
        /// </summary>
        static bool ConsoleEventCallback(int eventType)
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