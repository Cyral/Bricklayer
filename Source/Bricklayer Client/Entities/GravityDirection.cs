using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client.Entities
{
    /// <summary>
    /// Shows the different directions gravity can be pulling the player using gravity arrows
    /// </summary>
    [Flags]
    public enum GravityDirection
    {
        Default = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Float = 16,
    }
}
