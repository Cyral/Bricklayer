using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bricklayer.Server.Entities;
using Bricklayer.Common.World;

namespace Bricklayer.Server.World
{
    public class Map : Bricklayer.Common.World.Map
    {
        #region Properties
        /// <summary>
        /// The number of players online
        /// </summary>
        public override int Online { get { return this.Players.Count; } }
        #endregion

        /// <summary>
        /// Creates a server-side version of the map
        /// </summary>
        public Map(string name, string description, int width, int height, int id)
        {
            ID = id;
            Name = name;
            Description = description;
            Width = width;
            Height = height;
            IsServer = true; //Running a client
            Tiles = new Tile[Width, Height, 2];
            Players = new List<Bricklayer.Common.Entities.Player>();
            Generate();
        }

        #region Methods
        /// <summary>
        /// Generates a simple world with borders
        /// </summary>
        private void Generate()
        {
            //Temporary Generation
            int[] heightMap = new int[Width];

            //Config
            int offset = Height - 17;
            float peakheight = 5;
            float flatness = 40;
            int iterations = 8;

            double[] rands = new double[iterations];
            for (int i = 0; i < iterations; i++)
            {
                rands[i] = random.NextDouble() + i;
            }

            for (int x = 0; x < Width; x++)
            {
                double height = 0;
                for (int i = 0; i < iterations; i++)
                {
                    height += peakheight / rands[i] * Math.Sin((float)x / flatness * rands[i] + rands[i]);
                }
                heightMap[x] = (int)height + offset;
            }
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                        Tiles[x, y, 1] = new Tile(BlockType.Default);
                    else
                    {
                        if (y > heightMap[x] + 8)
                            Tiles[x, y, 1] = new Tile(BlockType.Stone);
                        else if (y > heightMap[x])
                            Tiles[x, y, 1] = new Tile(BlockType.Dirt);
                        else if (y == heightMap[x])
                            Tiles[x, y, 1] = new Tile(BlockType.Grass);
                        else
                            Tiles[x, y, 1] = new Tile(BlockType.Empty);
                    }

                    Tiles[x, y, 0] = new Tile(BlockType.Empty);

                }
            }
        }

        /// <summary>
        /// Returns a player from a RemoteUniqueIdentifier (The unique player network ID)
        /// </summary>
        public Player PlayerFromRUI(long RUI, bool ignoreError = false)
        {
            foreach (Player player in Players)
                if (player.RemoteUniqueIdentifier == RUI)
                    return player;
            if (!ignoreError)
                throw new KeyNotFoundException("Could not find player from RemoteUniqueIdentifier: " + RUI);
            else
                return null;
        }
        #endregion
    }
}
