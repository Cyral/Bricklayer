#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bricklayer.API;
using Newtonsoft.Json;
#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles disk operations such as saving maps and loading settings
    /// </summary>
    public class IO
    {
        #region Fields
        /// <summary>
        /// The current directory the server executable lies in
        /// Rather than loading from a static folder such as the client, the server
        /// Will load settings/maps from it's current folder, meaning you can drag and drop the server
        /// Into any folder to run it
        /// </summary>
        public static readonly string ServerDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //Files
        private static readonly string configFile = ServerDirectory + "\\server.config";
        private static readonly JsonSerializerSettings serializationSettings = 
            new JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented };
        #endregion

        #region Methods
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
                Server.Config = JsonConvert.DeserializeObject<Settings>(json);
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
        /// <summary>
        /// Gets an MD5 of a given file
        /// </summary>
        public static string GetFileMD5(string file)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var stream = File.OpenRead(file))
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }

        public static void LoadPlugins(Server server)
        {
            string[] files = Directory.GetFiles(Path.Combine(ServerDirectory, "Plugins"), "*.dll");

            List<string> loadedFiles = new List<String>(); //Temporary check for loaded files
            foreach (var file in files)
            {
                //Don't load duplicate files
                string md5 = GetFileMD5(file);
                if (!loadedFiles.Contains(md5))
                {
                    LoadPlugin(server, Path.Combine(Environment.CurrentDirectory, file));
                    loadedFiles.Add(md5);
                }
                else
                {
                    Program.WriteLine(string.Format("Duplicate plugin {0} ({1}) not loaded", Path.GetFileName(file), md5.Substring(0, 7)), ConsoleColor.Red);
                }
            }
         }

        private static void LoadPlugin(Server server, string file)
        {
            //Verify file name
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null))
                return;

            Assembly asm = null;

            //Load file
            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                Program.WriteLine("Unable to load " + file, ConsoleColor.Red);
            }

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Type pluginType = typeof(IPlugin);

                foreach (var t in types)
                {
                    if (pluginType.IsAssignableFrom((Type)t))
                    {
                        pluginInfo = t;
                        break;
                    }
                }

                //Create instance of the plugin and add it to the server
                if (pluginInfo != null)
                {
                    Object instance = Activator.CreateInstance(pluginInfo);
                    IPlugin plugin = (IPlugin)instance;
                    server.RegisterPlugin(plugin);
                }
            }
            catch (Exception ex)
            {
                Program.WriteLine(ex.ToString(), ConsoleColor.Red);
            }
        }
        #endregion

    }
}
