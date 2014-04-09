using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Networking.Messages
{
    public class JoinRoomMessage : IMessage
    {
        public string Name { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.JoinRoom; }
        }

        public JoinRoomMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public JoinRoomMessage(string name)
        {
            this.Name = name;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Name = im.ReadString();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Name);
        }
    }
}
