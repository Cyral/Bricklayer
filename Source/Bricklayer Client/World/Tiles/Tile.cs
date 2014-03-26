using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BricklayerClient.World
{
    /// <summary>
    /// Represents either a foreground or background tile in the map
    /// </summary>
    public class Tile
    {
        public const int Width = 16, Height = 16, DrawHeight = 20, DrawWidth = 20;
        public const int BackgroundIndex = 0, ForegroundIndex = 1;

        /// <summary>
        /// The type of block occupying the tile
        /// </summary>
        public BlockType Block
        {
            get { return block; }
            set { block = value; }
        }
        private BlockType block;

        /// <summary>
        /// The layer (foreground or background) the block occupies
        /// </summary>
        public Layer Layer { get { return block.Layer; } }

        /// <summary>
        /// Sets or creates a new block
        /// </summary>
        public Tile(BlockType block)
        {
            Block = block;
        }
        /// <summary>
        /// Handles updating the tile's logic
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            //No default behavior
        }
        /// <summary>
        /// Handles drawing of a single tile
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, int x, int y)
        {
            drawPosition.X = (x * Tile.Width);
            drawPosition.Y = ((y * Tile.Height) - (Tile.DrawHeight - Tile.Height)) + 1;
            spriteBatch.Draw(texture, drawPosition, Block.Source, Color.White);
        }
    }
}
