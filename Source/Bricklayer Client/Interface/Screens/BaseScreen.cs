using System;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    public class BaseScreen : IScreen
    {
        protected ScreenManager ScreenManager { get; private set; }
        protected MainWindow Window { get { return ScreenManager.Window; } }
        protected Manager Manager { get { return ScreenManager.Manager; } }
        public Action Initialized { get; set; }

        /// <summary>
        /// Adds the controls for this screen to the window
        /// </summary>
        public virtual void Add(ScreenManager screenManager)
        {
            if (Initialized != null)
            {
                Initialized();
            }
            ScreenManager = screenManager;
        }

        /// <summary>
        /// Removes the controls for this screen to the window
        /// </summary>
        public virtual void Remove()
        {

        }
    }
}
