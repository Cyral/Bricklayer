#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
#endregion

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// Manages transitions and states of game screens
    /// </summary>
    public class ScreenManager
    {
        #region Properties
        /// <summary>
        /// The main window instance (main game layout) 
        /// </summary>
        public MainWindow Window { get; set; }
        /// <summary>
        /// The NeoForce UI manager for the Window
        /// </summary>
        public Manager Manager { get; set; }
        /// <summary>
        /// The current screen playing
        /// </summary>
        public IScreen Current { get; set; }
        #endregion

        #region Fields
        private IScreen fadeTo;
        private ImageBox fadeImage;
        private Texture2D fadeTexture;
        private float fadeAlpha = 1;
        private FadeState state = FadeState.In;
        private enum FadeState { Idle, In, Out }
        #endregion

        #region Constants
        private const float fadeSpeed = 2f;
        #endregion

        public ScreenManager(MainWindow window)
        {
            Window = window;
            Manager = window.Manager;
            
            //Setup a solid black image to use for fading
            fadeTexture = new Texture2D(Manager.GraphicsDevice, 1, 1);
            fadeTexture.SetData<Color>(new Color[1] { Color.Black });

            fadeImage = new ImageBox(Manager) { Passive = true, Left = 0, Top = 0, Width = Window.Width, Height = Window.Height, StayOnTop = true, Image = fadeTexture, SizeMode = SizeMode.Stretched };
            fadeImage.Init();
            fadeImage.Image = fadeTexture;
            fadeImage.Alpha = 0;
            fadeImage.Color = Color.White * fadeImage.Alpha;
            window.Add(fadeImage);
            fadeImage.BringToFront();
            //window.Resize += new ResizeEventHandler(delegate(object o, ResizeEventArgs e)
            //{ fadeImage.SetSize(e.Width, e.Height); /* Update fade image size */ });
        }
        /// <summary>
        /// Transitions from the current screen to the next
        /// </summary>
        public void SwitchScreen(IScreen newScreen)
        {   
            //Do not fade if setting the login screen (first screen)
            if (!(newScreen is LoginScreen)) 
            {
                //Set the current screen and add it's objects
                fadeTo = newScreen;
                state = FadeState.Out;
            }
            else //If the screen is the first login screen
            {
                //Destory objects from the first screen
                if (Current != null)
                {
                    Current.Remove();
                }
                Current = newScreen;
                Current.Add(this);
            }
            fadeImage.BringToFront();
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state == FadeState.In)
            {
                fadeAlpha = MathHelper.Clamp(fadeAlpha - (elapsed * fadeSpeed), 0, 1);
                fadeImage.Alpha = fadeAlpha;
                fadeImage.Color = Color.White * fadeImage.Alpha;
                if (fadeAlpha <= 0)
                {
                    state = FadeState.Idle;
                }
            }
            else if (state == FadeState.Out)
            {
                fadeAlpha = MathHelper.Clamp(fadeAlpha + (elapsed * fadeSpeed), 0, 1);
                fadeImage.Alpha = fadeAlpha;
                fadeImage.Color = Color.White * fadeImage.Alpha;
                if (fadeAlpha >= 1) //Done fading to black, now set new screen and fade into it
                {
                    //Destory objects from the first screen
                    if (Current != null)
                    {
                        Current.Remove();
                    }
                    Current = fadeTo;
                    Current.Add(this);
                    state = FadeState.In;
                    fadeImage.BringToFront();
                }
            }
        }
        /// <summary>
        /// Forces a fade in
        /// </summary>
        public void FadeIn()
        {
            fadeAlpha = 1;
            state = FadeState.In;
            fadeImage.BringToFront();
        }
    }
}
