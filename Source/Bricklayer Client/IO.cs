#region Usings
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Bricklayer.Client.Networking;
using Newtonsoft.Json;
#endregion

namespace Bricklayer.Client
{
    public class IO
    {
        public static string AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static Dictionary<string, string> Directories = new Dictionary<string, string>();
        public static string MainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.Bricklayer";
        public const string MapSuffix = ".map";
        private static int FileBufferSize = 65536;

        public static List<ContentPack> ContentPacks = new List<ContentPack>();

        static IO()
        {
            //Add the sub-directories to the directory list
            Directories.Add("Maps", MainDirectory + "\\Maps\\");
            Directories.Add("Screenshots", MainDirectory + "\\Screenshots\\");
            Directories.Add("Content Packs", MainDirectory + "\\Content Packs\\");
        }
        /// <summary>
        /// Checks to make sure the application files are there, if not it will create them
        /// </summary>
        public static void CheckFiles()
        {
            //Check if the main directory exists. If its dosent, Create the main directory
            if (!Directory.Exists(MainDirectory))
                Directory.CreateDirectory(MainDirectory);
            //Now check for each sub-folder. If they dont exist, then add them
            foreach (KeyValuePair<string, string> kv in Directories)
                if (!Directory.Exists(kv.Value))
                    Directory.CreateDirectory(kv.Value);
        }
        /// <summary>
        /// Opens the settings file and loads the values
        /// </summary>
        public static void LoadSettings(Game game)
        {
            try
            {
                Game.Username = ConfigurationManager.AppSettings["Username"];
                Game.ContentPackName = ConfigurationManager.AppSettings["ContentPack"];
                string[] res = ConfigurationManager.AppSettings["Resolution"].Split(':');
                bool vsync = Boolean.Parse(ConfigurationManager.AppSettings["VSync"]);
                string hexColor = ConfigurationManager.AppSettings["Color"];
                Game.MyColor = Cyral.Extensions.Xna.ColorExtensions.ToColor(hexColor);

                Game.Resolution = new Microsoft.Xna.Framework.Rectangle(0,0,int.Parse(res[0]), int.Parse(res[1]));
                game.Graphics.PreferredBackBufferWidth = Game.Resolution.Width;
                game.Graphics.PreferredBackBufferHeight = Game.Resolution.Height;
                game.Manager.ScreenWidth = Game.Resolution.Width;
                game.Manager.ScreenHeight = Game.Resolution.Height;
                game.Graphics.SynchronizeWithVerticalRetrace = vsync;
                game.Graphics.ApplyChanges();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, game.Window.Title + " Configuration Error");
                //Default fallback settings
                Game.MyColor = Cyral.Extensions.Xna.ColorExtensions.ToColor("#FFBB00");
                Game.Resolution = new Microsoft.Xna.Framework.Rectangle(0, 0, 900, 600);
                game.Graphics.PreferredBackBufferWidth = Game.Resolution.Width;
                game.Graphics.PreferredBackBufferHeight = Game.Resolution.Height;
                game.Manager.ScreenWidth = Game.Resolution.Width;
                game.Manager.ScreenHeight = Game.Resolution.Height;
                game.Graphics.SynchronizeWithVerticalRetrace = false;
                game.Graphics.ApplyChanges();
            }
        }
        /// <summary>
        /// Finds content packs and loads the selected one
        /// </summary>
        public static void LoadContentPacks(Game game)
        {
            try
            {
                //Set current pack
                Game.ContentPackName = ConfigurationManager.AppSettings["ContentPack"];

                //Load Content Packs
                List<string> dirs = new List<string>();
                //Load embedded pack directories from Content folder
                dirs.Add(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Content\\Content Packs\\Default");
                //Load packs from Zarknorth folder
                dirs.AddRange(Directory.GetDirectories(Directories["Content Packs"]).ToList<string>());

                //Foreach directory
                for (int x = 0; x < dirs.Count(); x++)
                {
                    string dir = dirs[x];

                    //Load the packs xml data
                    XmlDocument doc = new XmlDocument();
                    doc.Load(dir + "\\pack.xml");

                    //Create the new pack and set it's data
                    ContentPack pack = new ContentPack();
                    pack.Name = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].Value;
                    pack.Description = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0].Value;
                    pack.Version = doc.ChildNodes[1].ChildNodes[2].ChildNodes[0].Value;
                    pack.Author = doc.ChildNodes[1].ChildNodes[3].ChildNodes[0].Value;
                    pack.Path = dir;
                    pack.Embedded = pack.Name == "Default";

                    //If an embedded pack, load from ContentManager, else FromStream
                    if (pack.Embedded)
                        pack.Icon = Game.TextureLoader.Content.Load<Texture2D>("Content\\Content Packs\\" + pack.Name + "\\icon");
                    else
                        using (FileStream fileStream = new FileStream(dir + "\\icon.png", FileMode.Open))
                            pack.Icon = Texture2D.FromStream(game.GraphicsDevice, fileStream);

                    //Add the pack
                    ContentPacks.Add(pack);

                    //If this pack is the current pack, set the game's data to it, so it is aware of the pack to use
                    if (pack.Name == Game.ContentPackName)
                    {
                        Game.ContentPackIndex = x;
                        Game.ContentPackData = pack;
                    }
                }
                //Load the current pack
                ContentPack.LoadPack(Game.ContentPackData);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }
        /// <summary>
        /// Read a list of servers from the json server config file
        /// </summary>
        public static List<ServerSaveData> ReadServers()
        {
            string fileName = MainDirectory + "\\servers.config";
            List<ServerSaveData> servers;
            if (!File.Exists(fileName))
            {
                //If server config does not exist, create it and write the default server to it
                WriteServers(new List<ServerSaveData>() { new ServerSaveData("Local Server", "127.0.0.1", 14242) }); 
            }
            string json = File.ReadAllText(fileName);
            servers = JsonConvert.DeserializeObject<List<ServerSaveData>>(json);
            return servers;
        }
        /// <summary>
        /// Save servers into a configurable json file
        /// </summary>
        public static void WriteServers(List<ServerSaveData> servers)
        {
            string fileName = MainDirectory + "\\servers.config";
            if (!File.Exists(fileName))
                File.Create(fileName);
            string json = JsonConvert.SerializeObject(servers);
            File.WriteAllText(fileName, json);
        }
    }
}
