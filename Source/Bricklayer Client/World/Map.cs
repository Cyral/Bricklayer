#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bricklayer.Client.Entities;
using Bricklayer.Client.World;
using Bricklayer.Common.Entities;
using Bricklayer.Common.World;
using Bricklayer.Common.Networking.Messages;
using Player = Bricklayer.Client.Entities.Player;
#endregion

namespace Bricklayer.Client.World
{
    public class Map : Bricklayer.Common.World.Map
    {
        #region Properties
        /// <summary>
        /// The main camera to follow the player
        /// </summary>
        public Camera MainCamera { get; set; }

        /// <summary>
        /// Reference to the game instance, if a client
        /// </summary>
        public Game Game { get; private set; }
        
        /// <summary>
        /// The minimap showing a preview of blocks
        /// </summary>
        public Minimap Minimap { get; set; }

        /// <summary>
        /// The currently selected block by the player
        /// </summary>
        public BlockType SelectedBlock { get; set; }
        /// <summary>
        /// The number of players online
        /// </summary>
        public override int Online { get { return this.Players.Count; } }
        #endregion

        #region Fields
        //Textures
        public Texture2D tileSheet, smileySheet, backgroundTexture, godTexture, bodyTexture, borderTexture;
        private const float cameraSpeed = .18f;
        private const float cameraParallax = .9f;
        private Texture2D pixel;
        #endregion

        /// <summary>
        /// Creates a client-side version of a map at the specified width and height (To be changed later once Init packet recieved)
        /// </summary>
        public Map(Game game, int width, int height) : base()
        {
            Game = game;
            Width = width;
            Height = height;
            //Running the client!
            IsServer = false;
            //Select default block
            SelectedBlock = BlockType.Default;
            //Initialize tile array
            Tiles = new Tile[Width, Height, 2];
            Players = new List<Bricklayer.Common.Entities.Player>();

            //Setup camera
            CreateCamera();
            LoadContent();

            Spawn = new Vector2(Tile.Width, Tile.Height);

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });

            SetMinimapColors();
        }

        /// <summary>
        /// Loads content needed for drawing on the client
        /// </summary>
        private void LoadContent()
        {
            tileSheet = ContentPack.Textures["map\\blocks"];
            //Background is it's own large texture to boost fps by drawing 1 large texture rather than hundreds of 16x16 background blocks per frame
            backgroundTexture = ContentPack.Textures["map\\background"];
            borderTexture = ContentPack.Textures["map\\border"];
            smileySheet = ContentPack.Textures["entity\\smileys"];
            bodyTexture = ContentPack.Textures["entity\\body"];
            godTexture = ContentPack.Textures["entity\\godmode"];
        }

        /// <summary>
        /// Creates a camera object at a set size
        /// </summary>
        public void CreateCamera()
        {
            if (Game != null)
                MainCamera = new Camera(new Vector2(Game.GraphicsDevice.Viewport.Width - Interface.GameScreen.SidebarWidth, Game.GraphicsDevice.Viewport.Height - 24)) {
                    MinBounds = new Vector2(-Game.GraphicsDevice.Viewport.Width, -Game.GraphicsDevice.Viewport.Height),
                    MaxBounds = new Vector2((Width * Tile.Width) + Game.GraphicsDevice.Viewport.Width, (Height * Tile.Height) + Game.GraphicsDevice.Viewport.Height)
                };
        }

        /// <summary>
        /// Updates the map's entities and handles input
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Game.IsActive && !Game.IsMouseOnControl)
                HandleInput();
            foreach (Player player in Players)
                player.Update(gameTime);

            //Follow Player
            MainCamera.Position = Vector2.Lerp(MainCamera.Position, Players[Game.MyIndex].PreviousState.Position - MainCamera.Origin, cameraSpeed * (elapsed * 60));
            MainCamera.Position = new Vector2((float)Math.Round(MainCamera.Position.X), (float)Math.Round(MainCamera.Position.Y));

            UpdateTiles(gameTime);
            Minimap.Update(gameTime);
        }

        /// <summary>
        /// Handles logic for updating tiles (Ex: animation)
        /// </summary>
        public void UpdateTiles(GameTime gameTime)
        {
            //Keep a tile variable handy so we don't have to make multiple requests to get one
            Tile tile;

            //Update Background Blocks
            for (int x = (int)MainCamera.Left / Tile.Width; x <= (int)MainCamera.Right / Tile.Width; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.Height); y >= (int)MainCamera.Top / Tile.Height; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y, 0];
                        if (tile.Block != BlockType.Empty)
                        {
                            tile.Update(gameTime);
                        }
                    }
                }
            }

            //Update Foreground Blocks
            for (int x = (int)MainCamera.Left / Tile.Width; x <= (int)MainCamera.Right / Tile.Width; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.Height); y >= (int)MainCamera.Top / Tile.Height; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y, 1];
                        if (tile.Block != BlockType.Empty)
                        {
                            tile.Update(gameTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles input for the level, such as clicking blocks and selecting them
        /// </summary>
        private void HandleInput()
        {
            //Get positions
            Point MousePosition = new Point((int)MainCamera.Position.X + (int)Game.Input.MousePosition.X, (int)MainCamera.Position.Y + (int)Game.Input.MousePosition.Y);
            Point GridPosition = new Point(MousePosition.X / Tile.Width, MousePosition.Y / Tile.Height);

            //If LeftButton Clicked
            if (Game.Input.IsLeftClicked())
            {

            }
            //If RightButton Clicked
            if (Game.Input.IsRightClicked())
            {

            }
            //If LeftButton Down
            if (Game.Input.IsLeftDown())
            {
                //Place a tile
                BlockType block = SelectedBlock;
                if (MousePosition.X > MainCamera.Left && MousePosition.Y > MainCamera.Top && MousePosition.X < MainCamera.Right && MousePosition.Y < MainCamera.Bottom)
                {
                    //Find the layer
                    Layer layer = Game.Input.IsKeyDown(Keys.LeftShift) && (SelectedBlock.Layer == Layer.Background || SelectedBlock.Layer == Layer.All) ? Layer.Background : Layer.Foreground;
                    PlaceTile(GridPosition.X, GridPosition.Y, layer, block, true); //Place the tile
                }
            }
            //If RightButton Down
            if (Game.Input.IsRightDown())
            {

            }

            if (!(Bricklayer.Client.Interface.MainWindow.ScreenManager.Current as Bricklayer.Client.Interface.GameScreen).ChatBox.TextBox.Focused && !Game.IsMouseOnControl)
            {

                //Select block
                int key = Game.Input.GetDigitPressed();

                BlockType[] Foregrounds = BlockType.BlockList.Where(x => x.Layer == Layer.Foreground || x.Layer == Layer.All).ToArray<BlockType>();
                if (key < Foregrounds.Length && key > -1)
                    SelectedBlock = Foregrounds[key];
            }
        }

        /// <summary>
        /// Draws the map blocks an entities
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw the background texture, wrapping it around the screen size
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, MainCamera.GetViewMatrix(Vector2.One));
            spriteBatch.Draw(borderTexture, (MainCamera.Position * new Vector2(cameraParallax, cameraParallax)) + new Vector2(-13, -3), new Rectangle((int)MainCamera.Left, (int)MainCamera.Top, (int)MainCamera.Right - (int)(MainCamera.Left * cameraParallax) + 16, (int)MainCamera.Bottom - (int)(MainCamera.Top * cameraParallax) + 3), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, MainCamera.GetViewMatrix(Vector2.One));
            //Draw border background
            Rectangle bgSource = new Rectangle(0, 0, Width * Tile.Width, Height * Tile.Height);
            spriteBatch.Draw(backgroundTexture, bgSource, Color.White);
            //Draw tiles
            DrawTiles(spriteBatch);
            //Draw Players
            foreach (Player player in Players)
                player.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            spriteBatch.Begin();
            Minimap.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the foreground and background blocks of a map
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            //Saves us from creating a new position for each block (Might create GC issues)
            Vector2 drawPosition = Vector2.Zero;
            //Keep a tile variable handy so we don't have to make multiple requests to get one
            Tile tile;
            //Draw Background Blocks
            for (int x = (int)MainCamera.Left / Tile.Width; x <= (int)MainCamera.Right / Tile.Width; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.Height); y >= (int)MainCamera.Top / Tile.Height; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y, 0];
                        if (tile.Block != BlockType.Empty)
                            tile.Draw(spriteBatch, tileSheet, drawPosition, x, y, Tile.BackgroundIndex);
                    }
                }
            }

            //Draw Foreground Blocks
            for (int x = (int)MainCamera.Left / Tile.Width; x <= (int)MainCamera.Right / Tile.Width; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.Height); y >= (int)MainCamera.Top / Tile.Height; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y, 1];
                        if (tile.Block != BlockType.Empty)
                            tile.Draw(spriteBatch, tileSheet, drawPosition, x, y, Tile.ForegroundIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the MainCamera.btnLeft or MainCamera.btnRight edges, but allowing things
        /// to jump beyond the MainCamera.Top of the level and fall off the MainCamera.bottom.
        /// </summary>
        public BlockCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (y < 1 || y >= Height - 1 || x < 1 || x >= Width - 1)
                return BlockCollision.Impassable;
            return Tiles[x, y, 1].Block.Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetTileBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Determines if a player can place a block at a specified position
        /// </summary>
        /// <returns>True if the player can place the block, false otherwise</returns>
        public bool CanPlaceBlock(int x, int y, int z, BlockType block)
        {
            bool Overlaps = false;
            bool InRange = InBounds(x, y);
            if (z == Tile.ForegroundIndex && block.ID != BlockType.Empty.ID)
            {
                Overlaps = Players.Any(p => (int)(p.SimulationState.Position.X / Tile.Width) == x &&
                     (int)(p.SimulationState.Position.Y / Tile.Height) == y);
            }
            return (!Overlaps && InRange);
        }

        #region Utilities
        /// <summary>
        /// Places a tile at the specified position, WHILE taking into account it's TileType
        /// Mainly used for player block placement, if you are looking to place a block through the code
        /// Either use `Tiles[x,y,z] =` or, set sendMessage to false
        /// </summary>
        /// <param name="x">The X position on the grid</param>
        /// <param name="y">The Y position on the grid</param>
        /// <param name="layer">The layer, either background or foreground</param>
        /// <param name="block">The block to place</param>
        /// <param name="sendMessage">Should the block be sent to the server or not</param>
        public void PlaceTile(int x, int y, Layer layer, BlockType block, bool sendMessage)
        {
            int z = layer == Layer.Foreground ? 1 : 0;
            if (CanPlaceBlock(x, y, z, block))
            {
                //If the block has changed, and we should send a message, send one
                if (sendMessage && Tiles[x, y, z].Block.ID != block.ID)
                {
                    Game.NetManager.Send(new BlockMessage(block, x, y, z));
                }
                //Set the block
                switch (block.Type)
                {
                    case TileType.Default:
                        Tiles[x, y, z] = new Tile(block);
                        break;
                    case TileType.Animated:
                        Tiles[x, y, z] = new AnimatedTile(block);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets block's default colors from their image for use with the minimap
        /// </summary>
        public void SetMinimapColors()
        {
            Texture2D texture = tileSheet;
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(data); //Get data 

            for (int i = 0; i < BlockType.BlockList.Count; i++)
            {
                BlockType block = BlockType.BlockList[i];
                int r = 0;
                int g = 0;
                int b = 0;
                int a = 0;
                int amount = 0;

                Rectangle source = new Rectangle(block.Source.X, 4, Tile.Width, Tile.Height);
                for (int c = 0; c < data.Length; c++) //Foreach colored pixel; Get RGB values
                {
                    int x = c % texture.Width;
                    int y = (c - x) / texture.Width;

                    if (block.Source.Contains(new Point(x, y)))
                    {
                        Color color = data[c];
                        if (color.A > 0)
                        {
                            r += color.R;
                            g += color.G;
                            b += color.B;
                            amount++;
                        }
                    }
                }
                //Set the block's minimap color based on the average color of the tile
                if (amount > 0)
                    block.Color = new Color(r / amount, g / amount, b / amount); //Calculate average
                else
                    block.Color = Color.Transparent;
            }
        }
        #endregion
    }
}
