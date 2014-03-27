using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Networking.Messages
{
    public class LoginMessage : IMessage
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Color Color { get; set; }

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
        public LoginMessage(string username, string password, Color color)
        {
            this.Username = username;
            this.Password = password;
            this.MessageTime = NetTime.Now;
            this.Color = color;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Username = im.ReadString();
            this.Password = im.ReadString();

            this.Color = new Color(im.ReadByte(), im.ReadByte(), im.ReadByte());
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Username);
            om.Write(this.Password);

            om.Write(this.Color.R);
            om.Write(this.Color.G);
            om.Write(this.Color.B);
        }
    }
}
