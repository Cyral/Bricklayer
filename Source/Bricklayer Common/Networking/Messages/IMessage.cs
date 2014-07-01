using Bricklayer.Common;
using Lidgren.Network;

namespace Bricklayer.Common.Networking.Messages
{
    public interface IMessage
    {
        MessageTypes MessageType { get; }

        void Decode(NetIncomingMessage im);
        void Encode(NetOutgoingMessage om);
    }
}