using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Common.Entities
{
    /// <summary>
    /// Represents the direction a player is facing
    /// </summary>
    public enum FacingDirection
    {
        Left,
        Right,
    }

    /// <summary>
    /// Represents a direction to check for world collisions
    /// </summary>
    public enum CollisionDirection
    {
        Vertical,
        Horizontal,
    }
}
