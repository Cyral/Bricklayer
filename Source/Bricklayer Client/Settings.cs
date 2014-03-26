using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client
{
    /// <summary>
    /// Contains settings to be serialized into JSON
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The name of the current content pack
        /// </summary>
        public string ContentPack { get; set; }
        /// <summary>
        /// The resolution, in pixels, of the game window
        /// </summary>
        public Microsoft.Xna.Framework.Point Resolution { get; set; }
        /// <summary>
        /// Temporary, the current username to be used
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The color, in hex, that the player should be tinted
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Determines if Vertical Synchronization should be enabled
        /// </summary>
        public bool UseVSync { get; set; }

        public static Settings GetDefaultSettings()
        {
            return new Settings()
            {
                ContentPack = "Default",
                Resolution = new Microsoft.Xna.Framework.Point(900,600),
                Username = "Guest",
                Color = "#FFBB00",
                UseVSync = false,
            };
        }
    }
}
