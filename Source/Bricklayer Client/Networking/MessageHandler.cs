using System.Diagnostics;
using System.Linq;
using Cyral.Extensions.Xna;
using Lidgren.Network;
using Bricklayer.Client.Entities;
using Bricklayer.Client.Interface;
using Bricklayer.Client.Networking.Messages;
using Bricklayer.Client.World;

namespace Bricklayer.Client.Networking
{
    /// <summary>
    /// Handles processing of incoming messages
    /// </summary>
    public class MessageHandler
    {
        private Game game; //Reference to the game class to interact with it
        //Quick access to game classes
        private NetworkManager NetManager { get {return Game.NetManager; }}
        private Map Map { get { return Game.Map; } set { Game.Map = value; } }
        private bool recievedInit = false; //Have we recieved the init message yet?

        public MessageHandler(Game game)
        {
            this.game = game;
        }
        /// <summary>
        /// The process network messages such as player's joining, moving, etc
        /// </summary>
        public void ProcessNetworkMessages()
        {
            NetIncomingMessage im; //Holder for the incoming message

            while ((im = NetManager.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Debug.WriteLine(im.ToString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                                //When connected to the server
                            case NetConnectionStatus.Connected:
                                {
                                    Debug.WriteLine("{0} Connected", im.SenderEndPoint);
                                    //Once connected, switch to the lobby screen
                                    MainWindow.ScreenManager.SwitchScreen(new LobbyScreen());
                                    Game.CurrentGameState = GameState.Lobby;
                                    break;
                                }
                                //When disconnected from the server
                            case NetConnectionStatus.Disconnected:
                                {
                                    Debug.WriteLine("{0} Disconnected", im.SenderEndPoint);
                                    if (Game.CurrentGameState != GameState.Login)
                                    {
                                        Game.CurrentGameState = GameState.Login;
                                        if (Game.CurrentGameState == GameState.Game)
                                        {
                                            (MainWindow.ScreenManager.Current as GameScreen).SystemChat("Server connection disconnected.");
                                            (MainWindow.ScreenManager.Current as GameScreen).StatsLabel.Text = "Status: Disconnected.";
                                        }
                                    }
                                    else
                                    {
                                        if (MainWindow.ScreenManager.Current is LoginScreen)
                                            (MainWindow.ScreenManager.Current as LoginScreen).Login.Disconnected();
                                    }
                                }
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                im.SenderConnection.Approve(NetManager.CreateMessage());
                                break;
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        HandleDataMessage(im);

                        break;
                }
                NetManager.Recycle(im);
            }
        }

        /// <summary>
        /// Handles a data message (The bulk of all messages recieved, containing player movements, block places, etc)
        /// </summary>
        private void HandleDataMessage(NetIncomingMessage im)
        {
            MessageTypes messageType = (MessageTypes)im.ReadByte(); //Find the type of data message sent
            switch (messageType)
            {
                case MessageTypes.Lobby: //Lobby list packet
                    {
                        if (Game.CurrentGameState == GameState.Lobby)
                        {
                            LobbyScreen screen = MainWindow.ScreenManager.Current as LobbyScreen;
                            LobbyMessage msg = new LobbyMessage(im);
                            screen.Name = msg.ServerName;
                            screen.Description = msg.Description;
                            screen.Intro = msg.Intro;
                            screen.Online = msg.Online;
                            screen.Rooms = msg.Rooms;
                            screen.Lobby.LoadRooms();
                        }
                        break;
                    }
                case MessageTypes.PlayerJoin: //Player join message
                    {
                        PlayerJoinMessage user = new PlayerJoinMessage(im);
                        //Add player to map
                        Map.Players.Add(new Player(Map, Map.Spawn, user.Username, 1, user.ID) { Tint = user.Color });
                        //Add user to userlist
                        (MainWindow.ScreenManager.Current as GameScreen).PlayerList.Items.Add(user.Username);
                        if (user.ID != Game.MyID && recievedInit) //Broadcast join message to chat
                        {
                            (MainWindow.ScreenManager.Current as GameScreen).SystemChat(user.Username + " [color:LightGray]has[/color] [color:LightGreen]joined.[/color]");
                        }
                        if (user.Me)
                        {
                            //Let game know of it's own player
                            Game.MyID = user.ID;
                            Game.MyIndex = (byte)Map.Players.IndexOf(Map.Players.First(x => x.ID == Game.MyID));
                            Game.Me.Tint = Game.MyColor;
                        }
                        break;
                    }
                case MessageTypes.PlayerLeave: //Player leave message
                    {
                        PlayerLeaveMessage user = new PlayerLeaveMessage(im);
                        //Remove player
                        if (user.ID != Game.MyID)
                        {
                            Player player = Map.PlayerFromID(user.ID);
                            Map.Players.Remove(Map.PlayerFromID(user.ID));
                            (MainWindow.ScreenManager.Current as GameScreen).PlayerList.Items.Remove(player.Username);
                            (MainWindow.ScreenManager.Current as GameScreen).SystemChat(player.Username + " [color:LightGray]has[/color] [color:IndianRed]left.[/color]");
                            //Rebuild indexes
                            for (int i = 0; i < Map.Players.Count; i++)
                            {
                                Map.Players[i].Index = i;
                                if (Map.Players[i].ID == Game.MyID)
                                    Game.MyIndex = (byte)i;
                            }
                        }
                        break;
                    }
                case MessageTypes.PlayerStatus: //Player move message
                    {
                        if (Game.CurrentGameState == GameState.Game)
                        {
                            PlayerStateMessage user = new PlayerStateMessage(im);
                            Player player = Map.PlayerFromID(user.ID);
                            player.SimulationState.Position = user.Position.ToVector2();
                            player.SimulationState.Velocity = user.Velocity.ToVector2();
                            player.SimulationState.Movement = user.Movement.ToVector2();
                            if (!recievedInit) //If we have not recieved init (meaning we are not in game yet), set the initial positions directly so interpolation doesn't mess with them
                                player.DisplayState.Position = player.SimulationState.Position;
                            player.VirtualJump = user.IsJumping;
                        }
                        break;
                    }
                case MessageTypes.Block: //Block place message
                    {
                        if (Game.CurrentGameState == GameState.Game)
                        {
                            BlockMessage msg = new BlockMessage(im);
                            BlockType block = BlockType.FromID(msg.BlockID);
                            if (Map.CanPlaceBlock(msg.X, msg.Y, msg.Z, block))
                                Map.Tiles[msg.X, msg.Y, msg.Z].Block = block;
                        }
                        break;
                    }
                case MessageTypes.PlayerSmiley: //Smiley change message
                    {
                        if (Game.CurrentGameState == GameState.Game)
                        {
                            PlayerSmileyMessage msg = new PlayerSmileyMessage(im);
                            Player p = Map.PlayerFromID(msg.ID);
                            p.Smiley = msg.Smiley;
                        }
                        break;
                    }
                case MessageTypes.PlayerMode: //Player mode change message
                    {
                        if (Game.CurrentGameState == GameState.Game)
                        {
                            PlayerModeMessage mode = new PlayerModeMessage(im);
                            Map.PlayerFromID(mode.ID).Mode = (PlayerMode)mode.Mode;
                        }
                        break;
                    }
                case MessageTypes.Chat: //Chat message
                    {
                        if (Game.CurrentGameState == GameState.Game)
                        {
                            ChatMessage chat = new ChatMessage(im);
                            (MainWindow.ScreenManager.Current as GameScreen).AddChat(Map.PlayerFromID(chat.ID).Username, chat.Message);
                        }
                        break;
                    }
                case MessageTypes.Init: //Initialization message with world data
                    {
                        Game.Map = new Bricklayer.Client.World.Map(game, 1, 1);
                        InitMessage msg = new InitMessage(im, Map);
                        Game.CurrentGameState = GameState.Game;
                        (MainWindow.ScreenManager.Current as GameScreen).Show();
                        (MainWindow.ScreenManager.Current as GameScreen).SystemChat("Connected to [color:LightGray]" + Game.Host + ":" + Game.Port + "[/color]");
                        recievedInit = true;
                        break;
                    }
            }
        }
    }
}
