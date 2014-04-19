using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.API
{
    /// <summary>
    /// The base of a plugin
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The description of the plugin
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The author of the plugin
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// The current version of the plugin
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// The plugin host
        /// </summary>
        public IPluginHost Host { get; set; }

        protected BasePlugin()
        {

        }

        public abstract void Load();
        public abstract void Unload();
    }
}
