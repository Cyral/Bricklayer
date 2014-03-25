using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BricklayerClient.World
{
    public class Tile
    {
        public const int WIDTH = 16, HEIGHT = 16, DRAWHEIGHT = 20, DRAWWIDTH = 20;

        public BlockType Foreground
        {
            get { return foreground; }
            set { foreground = value; }
        }
        private BlockType foreground;

        public BlockType Background
        {
            get { return background; }
            set { background = value; }
        }
        private BlockType background;

        public Tile(BlockType foreground, BlockType background)
        {
            Foreground = foreground;
            Background = background;
        }
        public Tile(BlockType foreground)
        {
            Foreground = foreground;
            Background = BlockType.Empty;
        }
    }
}
