using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BricklayerClient.Entities;
using Cyral.Extensions;
using BricklayerClient.Networking.Messages;

namespace BricklayerClient.Networking.Messages
{
    public class ChatMessage : IMessage
    {
        public byte ID { get; set; }

        public string Message { get; set; }

        public double MessageTime { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Chat; }
        }
        private static INetEncryption encrypter;
        private static string secret = "NfIkyQ9)o1y?BF6Won%?f9(c,a@~jt";
        public const int MaxLength = 80;
        static ChatMessage()
        {
            encrypter = new NetXorEncryption(secret);
        }
        public ChatMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }
        public ChatMessage(Player player,string message)
        {
            this.ID = player.ID;
            this.Message = message;
            this.MessageTime = NetTime.Now;
        }
        public void Decode(NetIncomingMessage im)
        {
            //im.Decrypt(encrypter);
            this.ID = im.ReadByte();
            this.Message = im.ReadString();
            if (Message.Length > Networking.Messages.ChatMessage.MaxLength)
                Message= Message.Truncate(Networking.Messages.ChatMessage.MaxLength);
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.ID);
            om.Write(this.Message);
            //om.Encrypt(encrypter);
        }
    }
}
