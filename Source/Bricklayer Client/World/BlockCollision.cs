using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client.World
{
    /// <summary>
    /// Represents the type of a collision a <c>BlockType</c> has, and how it interacts with a player
    /// </summary>
    public enum BlockCollision
    {
        Impassable,
        Passable,
        Platform,
        Gravity,
    }
}
