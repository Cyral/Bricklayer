using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    public class BaseScreen : IScreen
    {
        protected ScreenManager ScreenManager;
        protected MainWindow Window { get { return ScreenManager.Window; } }
        protected Manager Manager { get { return ScreenManager.Manager; } }

        /// <summary>
        /// Adds the controls for this screen to the window
        /// </summary>
        public virtual void Add(ScreenManager screenManager)
        {
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
