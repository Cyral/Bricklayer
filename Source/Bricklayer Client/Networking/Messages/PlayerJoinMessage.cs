using Lidgren.Network;

namespace BricklayerClient.Networking.Messages
{
    public class PlayerJoinMessage : IMessage
    {
        public string Username { get; set; }

        public byte ID { get; set; }

        public bool Me { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.PlayerJoin; }
        }

        public PlayerJoinMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public PlayerJoinMessage(string username, byte id, bool me)
        {
            this.Username = username;
            this.ID = id;
            this.Me = me;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Username = im.ReadString();
            this.ID = im.ReadByte();
            this.Me = im.ReadBoolean();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Username);
            om.Write(this.ID);
            om.Write(this.Me);
        }
    }
}
