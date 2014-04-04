using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bricklayer.Client.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bricklayer.Client.World
{
    /// <summary>
    /// Shows a small preview of a map
    /// </summary>
    public class Minimap
    {
        public Vector2 Position { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture2D Texture { get; private set; }

        private static Color emptyColor = Color.Black;
        private const float updateRate = 0f; //Time, in seconds, between map updates
        private double lastUpdate; //Time minimap was last updated
        private Map map; //Map reference
        private Color[] oneArray; //Array for holding processed 1D color data
        private Color[,] twoArray; //Array for holding raw 2D tile colors

        public Minimap(Map map, int maxWidth, int maxHeight)
        {
            this.map = map;
            Width = Math.Min(maxWidth, map.Width);
            Height = Math.Min(maxHeight, map.Height);
        }
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > lastUpdate + updateRate)
            {
                if (twoArray == null)
                {
                    //a 2D color array used to store the primary texture and allow precise data retrieval 
                    twoArray = new Color[Width, Height];
                    //the 1D array that will be used to create the final texture 
                    oneArray = new Color[Width * Height];
                }
                //Add tiles
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        Tile foreground = map.Tiles[x, y, 1];
                        Tile background = map.Tiles[x, y, 0];
                        //Foreground
                        if (foreground.Block.ID != BlockType.Empty.ID && foreground.Block.Color != Color.Transparent)
                        {
                            twoArray[x, y] = foreground.Block.Color;
                        }
                        //Background
                        else if (background.Block.ID != BlockType.Empty.ID && background.Block.Color != Color.Transparent)
                        {
                            //If BG and FG block, tint it darker slightly, otherwise draw it normally
                            if (background.Block.Layer == Layer.All)
                                twoArray[x, y] = Color.Lerp(background.Block.Color, Color.Black, .4f);
                            else
                            twoArray[x, y] = background.Block.Color;
                        }
                        else
                        {
                            twoArray[x, y] = emptyColor;
                        }

                        //Convert the 2D array to 1D
                        oneArray[x + y * Width] = twoArray[x % Width, y % Height];
                    }
                }
                //Add players
                foreach (Player player in map.Players)
                {
                    if (player.DisplayState.Position != player.PreviousState.Position && !player.LastColors.ContainsKey(player.GridPosition)) //Add a new fade color trail if they are moving
                    {
                        player.LastColors.Add(player.GridPosition, 0);
                    }
                    else if (player.LastColors.ContainsKey(player.GridPosition)) //If the color list already contains this point, update it
                    {
                        player.LastColors[player.GridPosition] = 0;
                    }
                    for (int i = 0; i < player.LastColors.Count; i++) //Fade out trail and add to map
                    {
                        KeyValuePair<Point, float> point = player.LastColors.ElementAt(i);
                        player.LastColors[point.Key] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        oneArray[point.Key.X + point.Key.Y * Width] = Color.Lerp(player.Tint, twoArray[point.Key.X, point.Key.Y], point.Value); //Fade between tint and original tile color
                        if (point.Value >= 1) //Remove old points
                        {
                            player.LastColors.Remove(point.Key);
                            i--;
                        }
                    }
                    oneArray[player.GridPosition.X + player.GridPosition.Y * Width] = player.Tint;
                }
                //Create a new, blank, square texture 
                if (Texture == null)
                    Texture = new Texture2D(map.Game.GraphicsDevice, Width, Height);

                //Assign the newly filled 1D array into the texture 
                Texture.SetData<Color>(oneArray);

                lastUpdate = gameTime.TotalGameTime.TotalSeconds;
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
