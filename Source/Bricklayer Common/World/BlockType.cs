using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bricklayer.Common.World;

namespace Bricklayer.Common.World
{
    /// <summary>
    /// A tile's block type (Ex: Dirt, Stone, etc)
    /// </summary>
    public class BlockType
    {
        /// <summary>
        /// List of all block types
        /// </summary>
        public static List<BlockType> BlockList;
        /// <summary>
        /// Name of the block
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// ID used for loading and saving
        /// </summary>
        public byte ID { get; private set; }
        /// <summary>
        /// The source rectangle the block is found in the sprite sheet
        /// </summary>
        public Rectangle Source { get; private set; }
        /// <summary>
        /// Texture
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Defines if this block is background or foreground
        /// </summary>
        public Layer Layer { get; set; }
        /// <summary>
        /// How players should collide with this tile, only for foregrounds
        /// </summary>
        public BlockCollision Collision { get; set; }
        /// <summary>
        /// The type of tile (Default, Animated, etc)
        /// </summary>
        public TileType Type { get; set; }
        /// <summary>
        /// The amount of time before frame changes, for animated tiles
        /// </summary>
        public float FrameTime { get; set; }
        /// <summary>
        /// The total amount of frames in an animation, for animated tiles
        /// </summary>
        public float TotalFrames { get; set; }
        /// <summary>
        /// The average color of the tile to appear on the minimap
        /// </summary>
        public Color Color { get; set; }
        public static BlockType Empty, Default, Stone, Dirt, Grass, Wood, Brick, Slab, Glass, UpArrow, RightArrow, DownArrow, LeftArrow;

        /// <summary>
        /// Creates a new instance of a <c>BlockType</c>
        /// </summary>
        /// <param name="name">Name of the block</param>
        public BlockType(string name, Layer layer, Rectangle source, BlockCollision collision = BlockCollision.Passable, TileType type = TileType.Default)
        {
            Name = name;
            Source = source;
            Layer = layer;
            Collision = collision;
            Type = type;
            TotalFrames = 1;
            ID = (byte)BlockList.Count();
            BlockList.Add(this);
        }
        static BlockType()
        {
            BlockList = new List<BlockType>();
            Init();
        }

        /// <summary>
        /// Adds/Creates all of the block type's
        /// </summary>
        public static void Init()
        {
            Empty = new BlockType("Empty", Layer.All, Rectangle.Empty);
            Default = new BlockType("Default", Layer.All, new Rectangle(0, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Stone = new BlockType("Stone", Layer.All, new Rectangle(Tile.DrawWidth, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Dirt = new BlockType("Dirt", Layer.All, new Rectangle(Tile.DrawWidth * 2, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Grass = new BlockType("Grass", Layer.All, new Rectangle(Tile.DrawWidth * 3, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Wood = new BlockType("Wood", Layer.All, new Rectangle(Tile.DrawWidth * 4, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Brick = new BlockType("Brick", Layer.All, new Rectangle(Tile.DrawWidth * 5, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Slab = new BlockType("Slab", Layer.All, new Rectangle(Tile.DrawWidth * 6, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            Glass = new BlockType("Glass", Layer.All, new Rectangle(Tile.DrawWidth * 7, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Impassable);
            UpArrow = new BlockType("Up Arrow", Layer.Foreground, new Rectangle(Tile.DrawWidth * 8, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Gravity);
            DownArrow = new BlockType("Down Arrow", Layer.Foreground, new Rectangle(Tile.DrawWidth * 10, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Gravity);
            LeftArrow = new BlockType("Left Arrow", Layer.Foreground, new Rectangle(Tile.DrawWidth * 9, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Gravity);
            RightArrow = new BlockType("Right Arrow", Layer.Foreground, new Rectangle(Tile.DrawWidth * 11, 0, Tile.DrawWidth, Tile.DrawHeight), BlockCollision.Gravity);
        }

        /// <summary>
        /// Finds a <c>BlockType</c> from it's ID
        /// </summary>
        public static BlockType FromID(byte ID)
        {
            return BlockList.First(x => x.ID == ID);
        }
    }
}
