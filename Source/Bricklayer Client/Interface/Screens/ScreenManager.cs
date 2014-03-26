using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    public class ScreenManager
    {
        public MainWindow Window;
        public Manager Manager;
        public IScreen Current;

        public ScreenManager(MainWindow window)
        {
            Window = window;
            Manager = window.Manager;
        }
        public void SwitchScreen(IScreen newScreen)
        {
            if (Current != null)
                Current.Remove();
            Current = newScreen;
            Current.Add(this);
        }
    }
}
