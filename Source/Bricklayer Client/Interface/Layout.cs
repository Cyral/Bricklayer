using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// Handles the layout aspects of the window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The main <c>ScreenManager</c> that handles control adding/removing
        /// </summary>
        public static ScreenManager ScreenManager { get; set; }
        public static Color DefaultTextColor = new Color(32, 32, 32);

        public MainWindow(Manager manager)
            : base(manager)
        {
            ElapsedTime = TimeSpan.Zero;
            Width = Game.Resolution.Width;
            Height = Game.Resolution.Height;
            Transparent = true;
            AutoScroll = false;

            //Set up the ScreenManager which will handle all of the controls from here
            ScreenManager = new ScreenManager(this);
            ScreenManager.SwitchScreen(new LoginScreen());
        }
    }
}
