using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Lidgren.Network;
using Bricklayer.Client.Networking.Messages;
using Timer = System.Timers.Timer;

namespace Bricklayer.Client.Networking
{
    /// <summary>
    /// Provides a simple to use interface for sending/recieving Lidgren messages
    /// </summary>
    public class NetworkManager
    {
        /// <summary>
        /// The underlying Lidgren NetClient.
        /// </summary>
        public NetClient Client { get; set; }
        private bool isDisposed; 

        /// <summary>
        /// Connects to a given server.
        /// </summary>
        public void Connect(string host, int port)
        {
            Game.Host = host;
            Game.Port = port;
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            NetPeerConfiguration Config = new NetPeerConfiguration("Bricklayer");

            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.EnableMessageType(NetIncomingMessageType.Data);

            // Create new client, with previously created configs
            Client = new NetClient(Config);

            // Start client
            Client.Start();

            Join(host, port);
        }

        /// <summary>
        /// Sends a message once connected to join a server officially.
        /// </summary>
        private void Join(string host, int port)
        {
            Client.Connect(new IPEndPoint(NetUtility.Resolve(host), port), EncodeMessage(new LoginMessage(Game.Username, "", Game.MyHue)));
        }

        /// <summary>
        /// Creates a NetOutgoingMessage from the interal Client object.
        /// </summary>
        /// <returns>A new NetOutgoingMessage.</returns>
        public NetOutgoingMessage CreateMessage()
        {
            return Client.CreateMessage();
        }

        /// <summary>
        /// Reads the latest message in the queue.
        /// </summary>
        /// <returns>The latest NetIncomingMessage, ready for processing, null if no message waiting.</returns>
        public NetIncomingMessage ReadMessage()
        {
            return Client.ReadMessage();
        }

        /// <summary>
        /// Sends and encodes an IMessage to the server.
        /// </summary>
        /// <param name="gameMessage">IMessage to write ID and send.</param>
        public void Send(IMessage gameMessage)
        {
            NetOutgoingMessage message = EncodeMessage(gameMessage); //Write packet ID and encode
            Client.SendMessage(message, NetDeliveryMethod.ReliableOrdered); //Send
        }

        /// <summary>
        /// Writes an IMessage's packet ID and encodes it's data into a NetOutgoingMessage.
        /// </summary>
        public NetOutgoingMessage EncodeMessage(IMessage gameMessage)
        {
            NetOutgoingMessage message = Client.CreateMessage();
            //Write packet type ID
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            return message;
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <param name="reason">Reason to tell the server for disconnecting.</param>
        public void Disconnect(string reason = "Bye")
        {
            Client.Disconnect(reason);
        }

        /// <summary>
        /// Gets the connection status from the internal client
        /// </summary>
        public NetConnectionStatus GetConnectionStatus()
        {
            return Client.ConnectionStatus;
        }
        /// <summary>
        /// Gets the server connection's Statistics from the internal client
        /// </summary>
        public NetConnectionStatistics GetConnectionStats()
        {
            return Client.ServerConnection.Statistics;
        }

        /// <summary>
        /// Gets the client status from the internal client
        /// </summary>
        /// <returns></returns>
        public NetPeerStatus GetPeerStatus()
        {
            return Client.Status;
        }

        /// <summary>
        /// Gets the client statistics from the internal client
        /// </summary>
        /// <returns></returns>
        public NetPeerStatistics GetPeerStats()
        {
            return Client.Statistics;
        }

        /// <summary>
        /// Recycles a message after processing by reusing it, reducing GC load
        /// </summary>
        /// <param name="im">Message to recylce</param>
        public void Recycle(NetIncomingMessage im)
        {
            Client.Recycle(im);
        }

        /// <summary>
        /// Disposes the NetworkManager
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }


        /// <summary>
        /// Disposes the connection and disconnects from the server
        /// </summary>
        /// <param name="disposing">Disconnect?</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }
                this.isDisposed = true;
            }
        }
    }
}
