using System;

namespace Bricklayer.Client.Interface
{
    public interface IScreen
    {
        Action Initialized { get; set; }
        GameState GameState { get; set; }
        void Add(ScreenManager screenManager);
        void Remove();
    }
}
