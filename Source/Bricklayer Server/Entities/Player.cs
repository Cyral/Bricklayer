using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bricklayer.Common.World;
using Microsoft.Xna.Framework;

namespace Bricklayer.Server.Entities
{
    public class Player : Bricklayer.Common.Entities.Player
    {
        /// <summary>
        /// The <c>RemoteUniqueIdentifier</c> for this player's connection
        /// </summary>
        public long RemoteUniqueIdentifier { get; set; }

        public Player(Map map, Vector2 position, string name, long remoteUniqueIdentifier, int id)
            : base(map, position, name, id)
        {
            RemoteUniqueIdentifier = remoteUniqueIdentifier;
        }
    }
}
