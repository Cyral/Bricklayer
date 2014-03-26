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
    public class NetworkManager
    {
        public NetClient Client;
        public delegate void ConnectedCallback();

        private const int ReconnectWait = 10000;
        private bool isDisposed;

        public NetworkManager()
        {

        }

        public void Connect(string host, int port, ConnectedCallback callback)
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
            callback();
        }

        private void Join(string host, int port)
        {
            Client.Connect(new IPEndPoint(NetUtility.Resolve(host), port), EncodeMessage(new LoginMessage(Game.Username, "default")));
        }
        public NetOutgoingMessage CreateMessage()
        {
            return Client.CreateMessage();
        }

        public NetIncomingMessage ReadMessage()
        {
            return Client.ReadMessage();
        }

        public void SendMessage(IMessage gameMessage)
        {
            NetOutgoingMessage message = Client.CreateMessage();
            //Write packet type ID
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            Client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public NetOutgoingMessage EncodeMessage(IMessage gameMessage)
        {
            NetOutgoingMessage message = Client.CreateMessage();
            //Write packet type ID
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            return message;
        }

        public void Disconnect(string reason = "Bye")
        {
            Client.Disconnect(reason);
        }
        public NetConnectionStatus GetConnectionStatus()
        {
            return Client.ConnectionStatus;
        }
        public NetConnectionStatistics GetConnectionStats()
        {
            return Client.ServerConnection.Statistics;
        }
        public NetPeerStatus GetPeerStatus()
        {
            return Client.Status;
        }
        public NetPeerStatistics GetPeerStats()
        {
            return Client.Statistics;
        }

        public void Recycle(NetIncomingMessage im)
        {
            Client.Recycle(im);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

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
