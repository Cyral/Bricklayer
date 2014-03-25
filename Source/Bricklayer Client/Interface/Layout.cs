using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace BricklayerClient.Interface
{
    public partial class MainWindow : Window
    {
        public static ScreenManager ScreenManager { get; set; }

        public MainWindow(Manager manager)
            : base(manager)
        {
            ElapsedTime = TimeSpan.Zero;
            Width = Game.Resolution.Width;
            Height = Game.Resolution.Height;
            Transparent = true;
            AutoScroll = false;

            ScreenManager = new ScreenManager(this);
            ScreenManager.SwitchScreen(new LoginScreen());
        }
    }
}
