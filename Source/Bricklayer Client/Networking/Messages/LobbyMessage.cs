using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Networking.Messages
{
    public class LobbyMessage : IMessage
    {
        public string ServerName { get; set; }

        public string Description { get; set; }

        public string Intro { get; set; }

        public byte Online { get; set; }


        public List<LobbySaveData> Rooms { get; set; }

        public MessageTypes MessageType
        {
            get { return MessageTypes.Lobby; }
        }

        public LobbyMessage(NetIncomingMessage im)
        {
            Rooms = new List<LobbySaveData>();
            this.Decode(im);
        }
        public LobbyMessage(string name, string description, string intro, int online, List<LobbySaveData> rooms)
        {
            this.ServerName = name;
            this.Description = description;
            this.Intro = intro;
            this.Rooms = rooms;
            this.Online = (byte)online;
        }
        public void Decode(NetIncomingMessage im)
        {
            this.ServerName = im.ReadString();
            this.Description = im.ReadString();
            this.Intro = im.ReadString();
            this.Online = im.ReadByte();
            int roomsLength = im.ReadByte();
            for (int i = 0; i < roomsLength; i++)
            {
                Rooms.Add(new LobbySaveData(im.ReadString(), im.ReadString(), im.ReadByte(), im.ReadInt16(), im.ReadDouble()));
            }
        }
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.ServerName);
            om.Write(this.Description);
            om.Write(this.Intro);
            om.Write(this.Online);
            om.Write((byte)Rooms.Count);
            for (int i = 0; i < Rooms.Count; i++)
            {
                om.Write(Rooms[i].Name);
                om.Write(Rooms[i].Description);
                om.Write((byte)Rooms[i].Online);
                om.Write((short)Rooms[i].Plays);
                om.Write(Rooms[i].Rating);
            }
        }
    }
}
