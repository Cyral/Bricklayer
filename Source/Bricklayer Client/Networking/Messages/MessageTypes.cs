namespace BricklayerClient.Networking.Messages
{
    public enum MessageTypes : byte
    {
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
    }
}
