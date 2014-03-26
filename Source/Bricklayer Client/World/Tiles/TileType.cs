using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BricklayerClient.World
{
    [Flags]
    public enum TileType
    {
        /// <summary>
        /// The default, standard block's tile type
        /// </summary>
        Default = 0,
        /// <summary>
        /// An animated tile's type
        /// </summary>
        Animated = 1,
    }
}
