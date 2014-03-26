using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bricklayer.Client.Entities;
using Bricklayer.Client.Networking.Messages;

namespace Bricklayer.Client.World
{
    public class Map
    {
        /// <summary>
        /// The tile array for the map, containing all tiles and tile data
        /// </summary>
        public Tile[,] Tiles { get; set; }

        //Width and Height
        public int Width { get { return width; } set { width = value; CreateCamera(); } }
        public int Height { get { return height; } set { height = value; CreateCamera(); } }
        private int width, height;

        /// <summary>
        /// The list of players currently in the map, synced with the server
        /// </summary>
        public List<Player> Players { get; set; }

        //Textures
        public Texture2D tileSheet, smileySheet, backgroundTexture, godTexture, bodyTexture;

        /// <summary>
        /// Defines if this map instance is part of a server
        /// </summary>
        public bool IsServer { get; set; }

        /// <summary>
        /// The main camera to follow the player
        /// </summary>
        public Camera MainCamera { get; set; }

        /// <summary>
        /// Reference to the game instance, if a client
        /// </summary>
        public Game Game { get; private set; }
        /// <summary>
        /// The spawn point new players will originate from
        /// </summary>
        public Vector2 Spawn { get; set; }

        /// <summary>
        /// Currently selected block to be placed
        /// </summary>
        public BlockType SelectedBlock { get; private set; }

        //Fields
        private const float cameraSpeed = .18f;
        
        /// <summary>
        /// Creates a client-side version of a map at the specified width and height (To be changed later once Init packet recieved)
        /// </summary>
        public Map(Game game, int width, int height)
        {
            Game = game;
            Width = width;
            Height = height;
            //Running the client!
            IsServer = false;
            //Select default block
            SelectedBlock = BlockType.Default;
            //Initialize tile array
            Tiles = new Tile[Width, Height];
            Players = new List<Player>();
            //Setup camera
            CreateCamera();
            LoadContent();
            Spawn = new Vector2(Tile.WIDTH, Tile.HEIGHT);
        }
        /// <summary>
        /// Creates a server-side version of the map
        /// </summary>
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            IsServer = true; //Running a client
            Tiles = new Tile[Width, Height];
            Players = new List<Player>();
            Generate();
        }
        /// <summary>
        /// Loads content needed for drawing on the client
        /// </summary>
        private void LoadContent()
        {
            tileSheet = ContentPack.Textures["map\\blocks"];
            //Background is it's own large texture to boost fps by drawing 1 large texture rather than hundreds of 16x16 background blocks per frame
            backgroundTexture = ContentPack.Textures["map\\background"];
            smileySheet = ContentPack.Textures["entity\\smileys"];
            bodyTexture = ContentPack.Textures["entity\\body"];
            godTexture = ContentPack.Textures["entity\\godmode"];
        }
        /// <summary>
        /// Creates a camera object at a set size
        /// </summary>
        private void CreateCamera()
        {
            if (Game != null) 
                MainCamera = new Camera(new Vector2(Game.GraphicsDevice.Viewport.Width - Interface.GameScreen.SidebarSize, Game.GraphicsDevice.Viewport.Height - 24)) { MinBounds = new Vector2(0, 0), MaxBounds = new Vector2(Width * Tile.WIDTH, (Height * Tile.HEIGHT)) };
        }
        /// <summary>
        /// Generates a simple world with borders
        /// </summary>
        private void Generate()
        {
            //Temporary Generation
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                        Tiles[x, y] = new Tile(BlockType.Default, BlockType.Empty);
                    else
                        Tiles[x, y] = new Tile(BlockType.Empty);
                }
            }
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
        }
        /// <summary>
        /// Handles input for the level, such as clicking blocks and selecting them
        /// </summary>
        private void HandleInput()
        {
            Point MousePosition = new Point((int)MainCamera.Position.X + Game.MousePoint.X, (int)MainCamera.Position.Y + Game.MousePoint.Y);
            Point GridPosition = new Point(MousePosition.X / Tile.WIDTH, MousePosition.Y / Tile.HEIGHT);
            
            //If LeftButton Clicked
            if (Game.MouseState.LeftButton == ButtonState.Pressed && Game.LastMouseState.RightButton == ButtonState.Released)
            {

            }
            //If RightButton Clicked
            if (Game.MouseState.RightButton == ButtonState.Pressed && Game.LastMouseState.RightButton == ButtonState.Released)
            {

            }
            //If LeftButton Pressed
            if (Game.MouseState.LeftButton == ButtonState.Pressed)
            {
                BlockType block = Game.KeyState.IsKeyDown(Keys.LeftShift) ? BlockType.Empty : SelectedBlock;
                if (CanPlaceBlock(GridPosition.X, GridPosition.Y, block) &&
                    MousePosition.X > MainCamera.Left && MousePosition.Y > MainCamera.Top && MousePosition.X < MainCamera.Right && MousePosition.Y < MainCamera.Bottom)
                {
                    Tiles[GridPosition.X, GridPosition.Y].Foreground = block;
                    Game.NetManager.SendMessage(new BlockMessage(Tiles[GridPosition.X, GridPosition.Y].Foreground, GridPosition.X, GridPosition.Y));
                }
            }
            //If RightButton Pressed
            if (Game.MouseState.RightButton == ButtonState.Pressed)
            {

            }

            if (!(Bricklayer.Client.Interface.MainWindow.ScreenManager.Current as Bricklayer.Client.Interface.GameScreen).ChatBox.TextBox.Focused && !Game.IsMouseOnControl)
            {
            //Select block
            int key = -1;

            if (Keys.D1.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 0;
            else if (Keys.D2.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 1;
            else if (Keys.D3.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 2;
            else if (Keys.D4.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 3;
            else if (Keys.D5.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 4;
            else if (Keys.D6.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 5;
            else if (Keys.D7.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 6;
            else if (Keys.D8.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 7;
            else if (Keys.D9.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 8;
            else if (Keys.D0.IsKeyToggled(Game.KeyState, Game.LastKeyState))
                key = 9;

            BlockType[] Foregrounds = BlockType.BlockList.Where(x => x.Layer == Layer.Foreground && x.ID != BlockType.Empty.ID).ToArray<BlockType>();
            if (key < Foregrounds.Length && key > -1)
                SelectedBlock = Foregrounds[key];
            }
        }
        /// <summary>
        /// Draws the map blocks an entities
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Saves us from creating a new position for each block (Might create GC issues)
            Vector2 drawPosition = Vector2.Zero;
            //Keep a tile variable handy so we don't have to make multiple requests to get one
            Tile tile;

            //Draw the background texture, wrapping it around the screen size
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, MainCamera.GetViewMatrix(Vector2.One));
            spriteBatch.Draw(backgroundTexture, MainCamera.Position + new Vector2(1,-3), new Rectangle((int)MainCamera.Left, (int)MainCamera.Top, (int)MainCamera.Right - (int)MainCamera.Left, (int)MainCamera.Bottom - (int)MainCamera.Top + 3), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None,0);
            spriteBatch.End();
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, MainCamera.GetViewMatrix(Vector2.One));
            //Draw Background Blocks
            for (int x = (int)MainCamera.Left / Tile.WIDTH; x <= (int)MainCamera.Right / Tile.WIDTH; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.HEIGHT); y >= (int)MainCamera.Top / Tile.HEIGHT; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y];
                        if (tile.Background != BlockType.Empty)
                        {
                            drawPosition.X = (x * Tile.WIDTH) + 3;
                            drawPosition.Y = ((y * Tile.HEIGHT) - (Tile.DRAWHEIGHT - Tile.HEIGHT)) + 1;
                            spriteBatch.Draw(tileSheet, drawPosition, tile.Background.Source, Color.White);
                        }
                    }
                }
            }

            //Draw Foreground Blocks
            for (int x = (int)MainCamera.Left / Tile.WIDTH; x <= (int)MainCamera.Right / Tile.WIDTH; x++)
            {
                for (int y = ((int)MainCamera.Bottom / Tile.HEIGHT); y >= (int)MainCamera.Top / Tile.HEIGHT; y--)
                {
                    if (InDrawBounds(x, y))
                    {
                        tile = Tiles[x, y];
                        if (tile.Foreground != BlockType.Empty)
                        {
                            drawPosition.X = (x * Tile.WIDTH) - 1;
                            drawPosition.Y = ((y * Tile.HEIGHT) - (Tile.DRAWHEIGHT - Tile.HEIGHT)) + 1;
                            spriteBatch.Draw(tileSheet, drawPosition, tile.Foreground.Source, Color.White);
                        }
                    }
                }
            }
            //Draw Players
            foreach (Player player in Players)
                player.Draw(spriteBatch, gameTime);
            spriteBatch.End();
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
            return Tiles[x, y].Foreground.Collision;
        }
        /// <summary>
        /// Determines if a grid position is in the bounds of the map
        /// </summary>
        public bool InBounds(int x, int y)
        {
            return !(y < 1 || y >= Height - 1 || x < 1 || x >= Width - 1);
        }
        /// <summary>
        /// Determines if a grid position is in the bounds of the drawing area (The map, but not the border)
        /// </summary>
        public bool InDrawBounds(int x, int y)
        {
            return !(y < 0 || y >= Height || x < 0 || x >= Width);
        }
        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.WIDTH, y * Tile.HEIGHT, Tile.WIDTH, Tile.HEIGHT);
        }
        /// <summary>
        /// Determines if a player can place a block at a specified position
        /// </summary>
        /// <returns>True if the player can place the block, false otherwise</returns>
        public bool CanPlaceBlock(int x, int y, BlockType block)
        {
            bool Overlaps = false;
            bool InRange = InBounds(x, y);
            if (block.Layer == Layer.Foreground && block.ID != BlockType.Empty.ID)
            {
                Overlaps = Players.Any(p => (int)(p.SimulationState.Position.X / Tile.WIDTH) == x &&
                     (int)(p.SimulationState.Position.Y / Tile.HEIGHT) == y);
            }
            return (!Overlaps && InRange);
        }

        #region Utilities
        /// <summary>
        /// Returns a player from an ID
        /// </summary>
        public Player PlayerFromID(int id)
        {
            foreach (Player player in Players)
                if (player.ID == id)
                    return player;
            throw new KeyNotFoundException("Could not find player from ID: " + id);
        }
        /// <summary>
        /// Returns a player from a RemoteUniqueIdentifier (The unique message ID)
        /// </summary>
        public Player PlayerFromRUI(long RUI, bool ignoreError = false)
        {
            foreach (Player player in Players)
                if (player.RUI == RUI)
                    return player;
            if (!ignoreError)
                throw new KeyNotFoundException("Could not find player from RemoteUniqueIdentifier: " + RUI);
            else
                return null;
        }
        #endregion
    }
}
