#region Usings
using System.Collections.Generic;
using System.Linq;
using Bricklayer.Common.Entities;
using Bricklayer.Common.Networking;
using Bricklayer.Common.Networking.Messages;
using Bricklayer.Common.World;
using Bricklayer.Common.Data;
using Lidgren.Network;
using Player = Bricklayer.Server.Entities.Player;
using System.Threading;
using System;
#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles network functions for the server, such as port-forwarding, sending messages, etc
    /// Send Function Guide:
    /// Send: Sends a message to a player
    /// Broadcast: Sends a message to each player in a map
    /// Global: Sends a message to each player on the server
    /// </summary>
    public class NetworkManager
    {
        #region Properties
        /// <summary>
        /// The underlying Lidgren server object
        /// </summary>
        public static NetServer NetServer { get; set; }
        /// <summary>
        /// The server configuration
        /// </summary>
        public static NetPeerConfiguration Config { get; set; }
        #endregion

        #region Fields
        private bool isDisposed = false; //Is the instance disposed?
        private const NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered; //Message delivery method
        #endregion

        #region Methods
        /// <summary>
        /// Starts the server connection
        /// </summary>
        /// <param name="port">Port to run on</param>
        /// <param name="maxconnections">Maximum clients connectable</param>
        public void Start(int port, int maxconnections)
        {
            //Set up config
            Config = new NetPeerConfiguration("Bricklayer"); //Network name
            Config.Port = port;
            Config.MaximumConnections = maxconnections;

            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.EnableMessageType(NetIncomingMessageType.Data);
            Config.EnableUPnP = true;

            NetServer = new NetServer(Config);
            NetServer.Start();

            //Attempt to port-forward over UPnP
            try { NetServer.UPnP.ForwardPort(port, "Bricklayer Port-Forwarding"); }
            catch (Exception e) { Log.WriteLine(e.ToString()); }
        }

        /// <summary>
        /// Creates and returns a query for a client request, with info like MOTD, online players, etc
        /// </summary>
        public ServerPingData GetQuery()
        {
            return new ServerPingData() {
                Online = NetServer.ConnectionsCount,
                MaxOnline = NetServer.Configuration.MaximumConnections,
                Description = Server.Config.Decription
            };
        }

        /// <summary>
        /// Creates a NetOutgoingMessage from the interal Server object
        /// </summary>
        /// <returns>A new NetOutgoingMessage</returns>
        public NetOutgoingMessage CreateMessage()
        {
            return NetServer.CreateMessage();
        }

        /// <summary>
        /// Reads the latest message in the queue
        /// </summary>
        /// <returns>The latest message, ready for processing, null if no message waiting</returns>
        public NetIncomingMessage ReadMessage()
        {
            return NetServer.ReadMessage();
        }

        /// <summary>
        /// Encodes and sends a message to a specified player
        /// </summary>
        /// <param name="gameMessage">IMessage to send</param>
        /// <param name="player">Player to send to</param>
        public void Send(IMessage gameMessage, Player player)
        {
            Send(gameMessage, (NetConnection)NetServer.Connections.Where(
                x => x.RemoteUniqueIdentifier == player.RemoteUniqueIdentifier &&
                Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map.ID == player.Map.ID)
               .ElementAt(0));
        }

        /// <summary>
        /// Encodes and sends a message to a specified NetConnection recipient
        /// </summary>
        /// <param name="gameMessage">IMessage to send</param>
        /// <param name="recipient">Client to send to</param>
        public void Send(IMessage gameMessage, NetConnection recipient)
        {
            NetServer.SendMessage(EncodeMessage(gameMessage), recipient, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Broadcasts a message to all players in a room, EXCEPT for the one specified
        /// </summary>
        /// <param name="gameMessage">IMessage to send</param>
        /// <param name="player">Player NOT to send to</param>
        public void BroadcastExcept(IMessage gameMessage, Player player)
        {
            BroadcastExcept(gameMessage,
                (NetConnection)NetServer.Connections.Where(x => x.RemoteUniqueIdentifier == player.RemoteUniqueIdentifier
                && Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map.ID == player.Map.ID)
                .ElementAt(0));
        }

        /// <summary>
        /// Broadcasts a message to all clients in a room, EXCEPT for the one specified
        /// </summary>
        /// <param name="gameMessage">IMessage to send</param>
        /// <param name="recipient">Client NOT to send to</param>
        public void BroadcastExcept(IMessage gameMessage, NetConnection recipient)
        {
            NetOutgoingMessage message = EncodeMessage(gameMessage);

            //Search for recipients
            List<NetConnection> recipients = NetServer.Connections.Where(
                x => x.RemoteUniqueIdentifier != recipient.RemoteUniqueIdentifier &&
                    Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true) != null &&
                    Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map.ID == 
                    Server.PlayerFromRUI(recipient.RemoteUniqueIdentifier).Map.ID)
                    .ToList<NetConnection>();

            if (recipients.Count > 0) //Send to recipients found
                NetServer.SendMessage(message, recipients, NetDeliveryMethod.ReliableOrdered, 0);
        }

        /// <summary>
        /// Broadcasts a message to all players in a room
        /// </summary>
        /// <param name="map">Map/Room to send to</param>
        /// <param name="gameMessage">IMessage to send</param>
        public void Broadcast(Map map, IMessage gameMessage)
        {
            NetOutgoingMessage message = EncodeMessage(gameMessage);

            //Search for recipients
            List<NetConnection> recipients = NetServer.Connections.Where(
                x => Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true) != null &&
                    Server.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map == map)
                    .ToList<NetConnection>();

            if (recipients.Count > 0) //Send to recipients found
                NetServer.SendMessage(message,recipients, NetDeliveryMethod.ReliableOrdered,0);
        }

        /// <summary>
        /// Sends a message to all players connected to the server
        /// </summary>
        /// <param name="gameMessage">IMessage to send</param>
        public void Global(IMessage gameMessage)
        {
            NetServer.SendToAll(EncodeMessage(gameMessage), deliveryMethod); //Send
        }

        /// <summary>
        /// Encodes a message with a packet ID so the client can identify what kind of message it is
        /// </summary>
        /// <param name="gameMessage">A message to encode</param>
        /// <returns>An encoded message as a NetOutgoingMessage</returns>
        public NetOutgoingMessage EncodeMessage(IMessage gameMessage)
        {
            NetOutgoingMessage message = NetServer.CreateMessage();
            //Write packet type ID
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            return message;
        }

        /// <summary>
        /// Shuts down the server and disconnects clients
        /// </summary>
        /// <param name="reason">Reason for shutting down</param>
        public void Shutdown(string reason = "Shutting Down")
        {
            NetServer.Shutdown(reason);
        }

        /// <summary>
        /// Recycles a message after processing by reusing it, reducing GC load
        /// </summary>
        /// <param name="im">Message to recylce</param>
        public void Recycle(NetIncomingMessage im)
        {
            NetServer.Recycle(im);
        }

        /// <summary>
        /// Disposes the NetworkManager
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the connection and shuts down the server
        /// </summary>
        /// <param name="disposing">Disconnect?</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Shutdown();
                }
                this.isDisposed = true;
            }
        }
        #endregion
    }
}
