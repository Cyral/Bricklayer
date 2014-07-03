﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Common
{
    public static class GlobalSettings
    {
        //Player name constants
        public const int MaxNameLength = 20;
        public const string NameRegex = "";

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
