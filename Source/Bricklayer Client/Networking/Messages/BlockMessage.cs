using Lidgren.Network;
using BricklayerClient.Entities;
using BricklayerClient.World;
using BricklayerClient.Networking.Messages;

namespace BricklayerClient.Networking.Messages
{
    public class BlockMessage : IMessage
    {
        public short X { get; set; }

        public short Y { get; set; }

        public byte BlockID { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Block; }
        }

        public BlockMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public BlockMessage(BlockType block, int x, int y)
        {
            this.BlockID = block.ID;
            this.X = (short)x;
            this.Y = (short)y;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.BlockID = im.ReadByte();
            this.X = im.ReadInt16();
            this.Y = im.ReadInt16();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.BlockID);
            om.Write(this.X);
            om.Write(this.Y);
        }
    }
}
