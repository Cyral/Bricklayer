using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Networking.Messages
{
    /// <summary>
    /// Used for requesting a packet from the server (Ex: Telling the server to send lobby data
    /// </summary>
    public class RequestMessage : IMessage
    {
        public MessageTypes RequestType { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Request; }
        }

        public RequestMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public RequestMessage(MessageTypes request)
        {
            this.RequestType = request;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.RequestType = (MessageTypes)im.ReadByte();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write((byte)RequestType);
        }
    }
}
