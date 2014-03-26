using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BricklayerClient.World
{
    /// <summary>
    /// Represents an instance of block that is able to be animated
    /// </summary>
    public class AnimatedTile : Tile
    {
        /// <summary>
        /// The current frame being played
        /// </summary>
        private byte frameIndex;
        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for
        /// TODO: Can we compress this further?
        /// </summary>
        private float time;

        public AnimatedTile(BlockType block) : base(block)
        {
            if (block.Type.HasFlag(TileType.Animated))
                Block = block;
            else
                throw new InvalidOperationException(block.Name + " is not an AnimatedTile");
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //If enough elapsed time has passed
            if (time >= Block.FrameTime)
            {
                //Reset timer and move to next frame
                time = 0;
                frameIndex++;
                if (frameIndex > Block.TotalFrames)
                    frameIndex = 0;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, int x, int y)
        {
            drawPosition.X = (x * Tile.Width);
            //Add the frameindex to change the Y coord on the spritesheet
            Rectangle source = Block.Source;
            source.Y += frameIndex * Tile.Height;
            drawPosition.Y = ((y * Tile.Height) - (Tile.DrawHeight - Tile.Height)) + 1;
            spriteBatch.Draw(texture, drawPosition, source, Color.White);
        }
    }
}
