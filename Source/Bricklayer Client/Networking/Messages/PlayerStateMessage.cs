using Cyral.Extensions.Xna;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Bricklayer.Client.Entities;

namespace Bricklayer.Client.Networking.Messages
{
    public class PlayerStateMessage : IMessage
    {
        //Positions only need to be points (ints) to save bandwidth, floating point accuracy is not needed
        public Point Position { get; set; }

        public Point Velocity { get; set; }

        public Point Movement { get; set; }

        public byte ID { get; set; }

        public bool IsJumping { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.PlayerStatus; }
        }

        public PlayerStateMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public PlayerStateMessage(Player player)
        {
            this.ID = player.ID;
            this.Position = player.SimulationState.Position.ToPoint();
            this.Velocity = player.SimulationState.Velocity.ToPoint();
            this.Movement = player.SimulationState.Movement.ToPoint();
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.ID = im.ReadByte();
            this.Position = new Point(im.ReadInt32(), im.ReadInt32());
            this.Velocity = new Point(im.ReadInt32(), im.ReadInt32());
            this.Movement = new Point(im.ReadInt32(), im.ReadInt32());
            this.IsJumping = im.ReadBoolean();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.ID);
            om.Write(this.Position.X);
            om.Write(this.Position.Y);
            om.Write(this.Velocity.X);
            om.Write(this.Velocity.Y);
            om.Write(this.Movement.X);
            om.Write(this.Movement.Y);
            om.Write(this.IsJumping);
        }
    }
}
