#region Usings
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles disk operations such as saving maps and loading settings
    /// </summary>
    public class IO
    {
        /// <summary>
        /// The current directory the server executable lies in
        /// Rather than loading from a static folder such as the client, the server
        /// Will load settings/maps from it's current folder, meaning you can drag and drop the server
        /// Into any folder to run it
        /// </summary>
        public static readonly string ServerDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //Files
        private static readonly string configFile = ServerDirectory + "\\server.config";
        private static readonly JsonSerializerSettings serializationSettings = new JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented };


        static IO()
        {
        }

        /// <summary>
        /// Opens the server settings and loads them into the server
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                //If server config does not exist, create it and write the default settings
                if (!File.Exists(configFile))
                    SaveSettings(Settings.GetDefaultSettings());
                string json = File.ReadAllText(configFile);
                //If config is empty, regenerate and read again
                if (string.IsNullOrWhiteSpace(json))
                    SaveSettings(Settings.GetDefaultSettings());
                json = File.ReadAllText(configFile);
                Program.Config = JsonConvert.DeserializeObject<Settings>(json);
            }
            catch (Exception ex)
            {
                throw; //TODO: Add some form of handling
            }
        }
        /// <summary>
        /// Saves server settings to the server config
        /// </summary>
        public static void SaveSettings(Settings settings)
        {
            try
            {
                //If server config does not exist, create it
                if (!File.Exists(configFile))
                {
                    FileStream str = File.Create(configFile);
                    str.Close();
                }
                string json = JsonConvert.SerializeObject(settings, serializationSettings);
                File.WriteAllText(configFile, json);
            }
            catch (Exception ex)
            {
                throw; //TODO: Add some form of handling
            }
        }
    }
}
