using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Bricklayer.Common.Networking.Messages
{
    public class JoinRoomMessage : IMessage
    {
        public int ID { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.JoinRoom; }
        }

        public JoinRoomMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public JoinRoomMessage(int id)
        {
            this.ID = id;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.ID = im.ReadInt16();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write((short)this.ID);
        }
    }
}
