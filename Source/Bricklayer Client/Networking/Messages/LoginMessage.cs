using Lidgren.Network;
using Microsoft.Xna.Framework;
using Cyral.Extensions.Xna;

namespace Bricklayer.Client.Networking.Messages
{
    public class LoginMessage : IMessage
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Color Color { get; set; }

        public long ID { get; set; }

        public double MessageTime { get; set; }

        public int Hue { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Login; }
        }

        public LoginMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public LoginMessage(string username, string password, int hue)
        {
            this.Username = username;
            this.Password = password;
            this.MessageTime = NetTime.Now;
            this.Hue = hue;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.Username = im.ReadString();
            this.Password = im.ReadString();

            this.Color = ColorExtensions.ColorFromHSV(im.ReadInt16(), Client.IO.ColorSaturation / 255f, Client.IO.ColorValue / 255f);
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Username);
            om.Write(this.Password);

            om.Write((short)this.Hue);
        }
    }
}
