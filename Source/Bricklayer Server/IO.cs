#region Usings
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BricklayerServer
{
    public class IO
    {
        public static string AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        static IO()
        {
        }

        /// <summary>
        /// Opens the settings file and loads the values
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                string port = ConfigurationManager.AppSettings["Port"];
                int.TryParse(port, out Program.Port);
                string max = ConfigurationManager.AppSettings["MaxPlayers"];
                int.TryParse(max, out Program.MaxConnections);
                Program.Motd = ConfigurationManager.AppSettings["MOTD"].Replace("\\n", Environment.NewLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert(false, ex.Message);
            }
        }
    }
}
