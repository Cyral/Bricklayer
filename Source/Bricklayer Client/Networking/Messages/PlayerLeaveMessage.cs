using Lidgren.Network;

namespace Bricklayer.Client.Networking.Messages
{
    public class PlayerLeaveMessage : IMessage
    {
        public byte ID { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.PlayerLeave; }
        }

        public PlayerLeaveMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public PlayerLeaveMessage(byte id)
        {
            this.ID = id;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.ID = im.ReadByte();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.ID);
        }
    }
}
