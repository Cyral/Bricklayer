namespace Bricklayer.Client.Networking.Messages
{
    public enum MessageTypes : byte
    {
        CreateRoom,
        JoinRoom,
        Request,
        PlayerStatus,
        PlayerJoin,
        PlayerLeave,
        PlayerSmiley,
        PlayerMode,
        Block,
        Login,
        Ping,
        Init,
        Chat,
        Lobby,
    }
}
