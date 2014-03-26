using Lidgren.Network;

namespace Bricklayer.Client.Networking.Messages
{
    public interface IMessage
    {
        MessageTypes MessageType { get; }

        void Decode(NetIncomingMessage im);
        void Encode(NetOutgoingMessage om);
    }
}