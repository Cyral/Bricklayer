﻿using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Bricklayer.Client.Entities;
using Bricklayer.Client.Networking;
using Bricklayer.Client.Networking.Messages;
using Bricklayer.Client.World;

namespace Bricklayer.Server
{
    public class NetworkManager
    {
        public static NetServer Server;
        public static NetPeerConfiguration Config;

        private bool isDisposed;

        public void Start(int port, int maxconnections)
        {
            Config = new NetPeerConfiguration("Bricklayer");
            Config.Port = port;
            Config.MaximumConnections = maxconnections;

            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.EnableMessageType(NetIncomingMessageType.Data);
            Config.EnableUPnP = true;

            Server = new NetServer(Config);
            Server.Start();
            //Attempt to port-forward over UPnP
            try { Server.UPnP.ForwardPort(port, "Bricklayer Port-Forwarding"); }
            catch { }
            
        }
        public ServerPingData GetQuery()
        {
            return new ServerPingData() { Online = Server.ConnectionsCount, MaxOnline = Server.Configuration.MaximumConnections, Description = Program.Config.Decription };
        }
        public NetOutgoingMessage CreateMessage()
        {
            return Server.CreateMessage();
        }

        public NetIncomingMessage ReadMessage()
        {
            return Server.ReadMessage();
        }
        public void SendMessage(IMessage gameMessage, Player player)
        {
            SendMessage(gameMessage, (NetConnection)Server.Connections.Where(x => x.RemoteUniqueIdentifier == player.RUI && Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map == player.Map).ElementAt(0));
        }
        public void SendMessage(IMessage gameMessage, NetConnection recipient)
        {
            NetOutgoingMessage message = Server.CreateMessage();
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            Server.SendMessage(message, recipient, NetDeliveryMethod.ReliableOrdered);
        }
        public void BroadcastMessageButPlayer(IMessage gameMessage, Player player)
        {
            BroadcastMessageButPlayer(gameMessage, (NetConnection)Server.Connections.Where(x => x.RemoteUniqueIdentifier == player.RUI && Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map == player.Map).ElementAt(0));
        }
        public void BroadcastMessageButPlayer(IMessage gameMessage, NetConnection recipient)
        {
            NetOutgoingMessage message = Server.CreateMessage();
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            List<NetConnection> recipients = Server.Connections.Where(x => x.RemoteUniqueIdentifier != recipient.RemoteUniqueIdentifier && Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true) != null && Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map == Program.PlayerFromRUI(recipient.RemoteUniqueIdentifier).Map).ToList<NetConnection>();
            if (recipients.Count > 0)
                Server.SendMessage(message, recipients, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void BroadcastMessage(Map map, IMessage gameMessage)
        {
            NetOutgoingMessage message = Server.CreateMessage();
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            List<NetConnection> recipients = Server.Connections.Where(x => Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true) != null && Program.PlayerFromRUI(x.RemoteUniqueIdentifier, true).Map == map).ToList<NetConnection>();
            if (recipients.Count > 0)
                Server.SendMessage(message,recipients, NetDeliveryMethod.ReliableOrdered,0);
        }

        public NetOutgoingMessage EncodeMessage(IMessage gameMessage)
        {
            NetOutgoingMessage message = Server.CreateMessage();
            //Write packet type ID
            message.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(message);
            return message;
        }

        public void Disconnect(string reason = "Shutting Down")
        {
            Server.Shutdown(reason);
        }

        public void Recycle(NetIncomingMessage im)
        {
            Server.Recycle(im);
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
