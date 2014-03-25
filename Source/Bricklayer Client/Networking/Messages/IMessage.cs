using Lidgren.Network;

namespace BricklayerClient.Networking.Messages
{
    public interface IMessage
    {
        MessageTypes MessageType { get; }

        void Decode(NetIncomingMessage im);
        void Encode(NetOutgoingMessage om);
    }
}