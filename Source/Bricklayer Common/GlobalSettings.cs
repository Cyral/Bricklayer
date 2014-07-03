using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bricklayer.Common
{
    public static class GlobalSettings
    {
        //Player name constants
        public static readonly Regex NameRegex = new Regex("^[a-zA-Z0-9_-]{3,16}$");

        //Player color constants
        public const int
            ColorValue = 250,
            ColorSaturation = 210;

        //Connection constants
        public const int
             ConnectTimeout = 3,
             DefaultPort = 14242;
    }
}
