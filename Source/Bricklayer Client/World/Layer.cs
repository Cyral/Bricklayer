using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client.World
{
    /// <summary>
    /// Represents a layer for tiles to be drawn on
    /// </summary>
    public enum Layer : byte
    {
        Foreground,
        Background,
        All,
    }
}
