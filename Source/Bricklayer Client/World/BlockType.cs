using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bricklayer.Client.World;

namespace Bricklayer.Client.World
{
    /// <summary>
    /// Block Type
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

        public static BlockType Empty, Default, Stone, Dirt, Grass, Wood, Brick, Slab, Glass;

        /// <summary>
        /// Creates a new instance of a blocktype
        /// </summary>
        /// <param name="name">Name of the block</param>
        public BlockType(string name, Layer layer, Rectangle source, BlockCollision collision = BlockCollision.Passable)
        {
            Name = name;
            Source = source;
            Layer = layer;
            Collision = collision;
            ID = (byte)BlockList.Count();
            BlockList.Add(this);
        }
        static BlockType()
        {
            BlockList = new List<BlockType>();
            Init();
        }
        /// <summary>
        /// Add all of the achivement configs here
        /// </summary>
        public static void Init()
        {
            Empty = new BlockType("Empty", Layer.Foreground, Rectangle.Empty);
            Default = new BlockType("Default", Layer.Foreground, new Rectangle(0, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Stone = new BlockType("Stone", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Dirt = new BlockType("Dirt", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 2, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Grass = new BlockType("Grass", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 3, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Wood = new BlockType("Wood", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 4, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Brick = new BlockType("Brick", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 5, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Slab = new BlockType("Slab", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 6, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
            Glass = new BlockType("Glass", Layer.Foreground, new Rectangle(Tile.DRAWWIDTH * 7, 0, Tile.DRAWWIDTH, Tile.DRAWHEIGHT), BlockCollision.Impassable);
        }
        public static BlockType FromID(byte ID)
        {
            return BlockList.First(x => x.ID == ID);
        }
    }
}
