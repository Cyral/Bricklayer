using Lidgren.Network;
using Bricklayer.Client.Entities;

namespace Bricklayer.Client.Networking.Messages
{
    public class PlayerModeMessage : IMessage
    {
        public PlayerMode Mode {get; set;}

        public byte ID { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.PlayerMode; }
        }

        public PlayerModeMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public PlayerModeMessage(Player player)
        {
            this.Mode = player.Mode;
            this.ID = player.ID;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Mode = (PlayerMode)im.ReadByte();
            this.ID = im.ReadByte();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write((byte)this.Mode);
            om.Write(this.ID);
        }
    }
}
