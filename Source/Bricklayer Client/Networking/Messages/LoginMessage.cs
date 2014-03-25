using Lidgren.Network;

namespace BricklayerClient.Networking.Messages
{
    public class LoginMessage : IMessage
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public long ID { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Login; }
        }

        public LoginMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public LoginMessage(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Username = im.ReadString();
            this.Password = im.ReadString();
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Username);
            om.Write(this.Password);
        }
    }
}
