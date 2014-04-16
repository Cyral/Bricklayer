using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Entities
{
    /// <summary>
    /// Represents the current state of an entity (It's position, velocity, etc)
    /// </summary>
    public struct EntityState
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Movement;
        public Rectangle Bounds;
    }
}
