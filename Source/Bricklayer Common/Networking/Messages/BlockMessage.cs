using Lidgren.Network;
using Bricklayer.Common.Entities;
using Bricklayer.Common.World;
using Bricklayer.Common.Networking.Messages;

namespace Bricklayer.Common.Networking.Messages
{
    public class BlockMessage : IMessage
    {
        /// <summary>
        /// X grid position of the placed block
        /// </summary>
        public short X { get; set; }

        /// <summary>
        /// Y grid position of the placed block
        /// </summary>
        public short Y { get; set; }

        public byte Z { get; set; }

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
        public BlockMessage(BlockType block, int x, int y, int z)
        {
            if (z > 1) //Throw an exception if the Z is out of range
                throw new System.ArgumentOutOfRangeException("The Z coordinate cannot be more than 1");

            this.BlockID = block.ID;
            this.X = (short)x;
            this.Y = (short)y;
            this.Z = (byte)z;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.BlockID = im.ReadByte();
            this.X = im.ReadInt16();
            this.Y = im.ReadInt16();
            this.Z = im.ReadByte();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.BlockID);
            om.Write(this.X);
            om.Write(this.Y);
            om.Write(this.Z);
        }
    }
}
