using Lidgren.Network;
using Bricklayer.Common.Entities;

namespace Bricklayer.Common.Networking.Messages
{
    public class PlayerSmileyMessage : IMessage
    {
        public byte ID { get; set; }

        public SmileyType Smiley { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.PlayerSmiley; }
        }

        public PlayerSmileyMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        public PlayerSmileyMessage(Player player, SmileyType smiley)
        {
            this.ID = player.ID;
            this.Smiley = smiley;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.ID = im.ReadByte();
            this.Smiley = SmileyType.FromID(im.ReadByte());
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.ID);
            om.Write(Smiley.ID);
        }
    }
}
