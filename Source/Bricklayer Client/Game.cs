using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cyral.Extensions.Xna;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BricklayerClient.Entities;
using BricklayerClient.Interface;
using BricklayerClient.Networking;
using BricklayerClient.Networking.Messages;
using BricklayerClient.World;
using TomShane.Neoforce.Controls;

namespace BricklayerClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Application
    {
        //Settings
        public static Rectangle Resolution;
        public static ContentPack ContentPackData;
        public static string ContentPackName;
        public static int ContentPackIndex;

        //Networking
        public static NetworkManager NetManager;
        public static MessageHandler MsgHandler;

        public static SpriteFont DefaultFont;
        public static TextureLoader TextureLoader;
        public static MainWindow MainWindow;
        public static GameState CurrentGameState = GameState.Loading;

        public static Map Map;
        public static Random Random;

        //Data
        public static string Host;
        public static int Port;
        public static string Username;
        public static byte MyID;
        public static byte MyIndex;
        public static Player Me { get { return Map.Players[MyIndex]; } }
        public static Color MyColor;

        //Input
        public static MouseState MouseState, LastMouseState;
        public static KeyboardState KeyState, LastKeyState;
        public static Point MousePoint;
        public static bool IsMouseOnControl;

        private SpriteBatch spriteBatch;

        public Game(): base("Bricklayer",true)
        {
            //Set up the window and UI defaults
            Manager.Content.RootDirectory = "Content";
            Manager.SkinDirectory = "Content/Skins";

            Window.Title = "Bricklayer - " + AssemblyVersionName.GetVersion();

            SystemBorder = true;
            FullScreenBorder = false;
            ExitConfirmation = false;
            ClearBackground = true;
            Manager.TargetFrames = 60;
            Manager.UseGuide = false;

            Random = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            spriteBatch = Manager.Renderer.SpriteBatch as SpriteBatch; //Set the spritebatch to the Neoforce managed one
            DefaultFont = Manager.Skin.Fonts["Default8"].Resource; //Default font we can use for drawing later

            NetManager = new NetworkManager();
            MsgHandler = new MessageHandler(this);
        }
        /// <summary>
        /// Creates a new instance of the main UI window
        /// </summary>
        protected override Window CreateMainWindow()
        {
            TextureLoader = new TextureLoader(Manager.GraphicsDevice, Content);
            IO.CheckFiles();
            IO.LoadContentPacks(this); //Load textures from content pack
            IO.LoadSettings(this);
            MainWindow = new Interface.MainWindow(Manager);
            return MainWindow;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //BlockType.LoadContent();
        }
        public override void Exit()
        {
            //Make sure to explicitly dispose of the network manager so the server knows we have disconnected
            if (NetManager.Client != null)
            {
                NetManager.Disconnect("Client Closed.");
                NetManager.Dispose();
            }
            base.Exit();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (NetManager.Client != null)
                MsgHandler.ProcessNetworkMessages();
            LastKeyState = KeyState;
            LastMouseState = MouseState;
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            MousePoint = MouseState.GetPositionPoint();
            IsMouseOnControl = !CheckPosition(MousePoint);
            switch (CurrentGameState)
            {
                case GameState.Loading:
                    {

                        break;
                    }
                case GameState.Game:
                    {
                        Map.Update(gameTime);
                        break;
                    }
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        /// <summary>
        /// Draws content behind the UI interface
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void DrawScene(GameTime gameTime)
        {
            switch (CurrentGameState)
            {
                case GameState.Loading:
                    {
                        break;
                    }
                case GameState.Game:
                    {
                        MainWindow.TotalFrames++;
                        Map.Draw(spriteBatch, gameTime);
                        break;
                    }
            }
            base.DrawScene(gameTime);
        }

        #region UI Extras
        /// <summary>
        /// Checks to see if the mouse is over a control (Mostly used for seeing if you can place a block there)
        /// </summary>
        public bool CheckPosition(Point pos)
        {
            if (CurrentGameState == GameState.Game && (MainWindow.ScreenManager.Current as GameScreen).ChatBox.TextBox.Focused)
                return false;
            // Is the mouse cursor within the application window?
            if (pos.X >= 0 && pos.X <= Manager.TargetWidth && pos.Y >= 24 && pos.Y <= Manager.TargetHeight)
            {
                foreach (Control c in Manager.Controls)
                {
                    if (CurrentGameState == GameState.Game && c == (MainWindow.ScreenManager.Current as GameScreen).ChatBox)
                        continue;
                    if (!CheckControlPos(c, pos))
                        return false;
                }
                // Mouse is not over any controls, but is within the application window.
                return true;
            }
            else
                return false;
        }
        public bool CheckControlPos(Control c, Point pos)
        {
            // Is this a visible control other than the MainWindow?
            // Is the mouse cursor within this control's boundaries?
            if (c.Visible && !c.Passive && c != MainWindow &&
                pos.X >= c.AbsoluteRect.Left && pos.X <= c.AbsoluteRect.Right &&
                pos.Y >= c.AbsoluteRect.Top && pos.Y <= c.AbsoluteRect.Bottom)
            {
                // Yes, mouse cursor is over this control.
                return false;
            }
            else
                return true;
        }
        #endregion

    }
}
