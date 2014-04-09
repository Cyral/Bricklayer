using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bricklayer.Client.Entities
{
    public struct EntityState
    {
        public Vector2 Position;
        private Vector2 position;
        public Vector2 Velocity;
        public Vector2 Movement;
        public Rectangle Bounds;
    }
}
