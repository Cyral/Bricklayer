#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Bricklayer.API;
using Bricklayer.Common.World;
using Bricklayer.Server.World;
using Newtonsoft.Json;
#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles disk saving/loading operations
    /// </summary>
    public class IO
    {
        #region Fields
        /// <summary>
        /// The current directory the server executable lies in
        /// rather than loading from a static folder such as the client, the server
        /// will load settings/maps from it's current folder, meaning you can drag and drop the server
        /// into any folder to run it
        /// </summary>
        public static readonly string ServerDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //Files
        private static int fileBufferSize = 65536;
        private static readonly string mapSuffix = ".blmap";
        private static readonly string pluginFolder = Path.Combine(ServerDirectory, "Plugins");
        private static readonly string mapFolder = Path.Combine(ServerDirectory, "Maps");
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
            if (!Directory.Exists(pluginFolder))
                Directory.CreateDirectory(pluginFolder);
            string[] files = Directory.GetFiles(pluginFolder, "*.dll");

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
                    Log.WriteLine(string.Format("Duplicate plugin {0} ({1}) not loaded", Path.GetFileName(file), md5.Substring(0, 7)), ConsoleColor.Red);
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
                Log.WriteLine(LogType.Error, "Unable to load {0}", file);
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
                Log.WriteLine(ex.ToString(), ConsoleColor.Red);
            }
        }

        public static void SaveMaps()
        {
            foreach (Bricklayer.Server.World.Map map in Server.Maps)
                SaveMap(map);
        }

        public static void LoadMaps()
        {
            string[] mapFiles = Directory.GetFiles(mapFolder, "*" + mapSuffix);
            foreach (string file in mapFiles)
                Server.Maps.Add(LoadMap(Path.GetFileNameWithoutExtension(file)));
        }

        public static Bricklayer.Server.World.Map LoadMap(string name)
        {
            Bricklayer.Server.World.Map map;
            using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                new BufferedStream(
                File.Open(Path.Combine(mapFolder, name + mapSuffix), FileMode.Open))))
            {
                //Read general data
                map = new Bricklayer.Server.World.Map(binaryReader.ReadString(), binaryReader.ReadString(),
                    binaryReader.ReadInt16(), binaryReader.ReadInt16(), Server.Maps.Count) 
                    { Rating = binaryReader.ReadDouble() };

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        Tile fg;
                        Tile bg;
                        SaveFlags sf = (SaveFlags)binaryReader.ReadByte();

                        int RLE = 0;
                        if (sf.HasFlag(SaveFlags.RLE))
                            RLE = binaryReader.ReadInt16();
                        if (sf.HasFlag(SaveFlags.Foreground))
                            fg = new Tile(BlockType.FromID(binaryReader.ReadByte()));
                        else
                            fg = new Tile(BlockType.Empty);
                        if (sf.HasFlag(SaveFlags.Background))
                            bg = new Tile(BlockType.FromID(binaryReader.ReadByte()));
                        else
                            bg = new Tile(BlockType.Empty);

                        map.Tiles[x, y, 1] = fg;
                        map.Tiles[x, y, 0] = bg;

                        if (RLE > 0) //RLE enabled
                        {
                            for (int i = 1; i <= RLE; i++)
                            {
                                 map.Tiles[x + i, y, 1] = fg;
                                 map.Tiles[x + i, y, 0] = bg;
                            }
                            x += RLE;
                        }
                    }
                }
            }
            return map;
        }

        public static void SaveMap(Bricklayer.Server.World.Map map)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter( //Create a new binary writer to write to the file
                new BufferedStream(
                File.Open(Path.Combine(mapFolder, map.Name + mapSuffix), FileMode.Create))))
            {
                //Write general data
                binaryWriter.Write(map.Name);
                binaryWriter.Write(map.Description);
                binaryWriter.Write((short)map.Width);
                binaryWriter.Write((short)map.Height);
                binaryWriter.Write(map.Rating);

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        BlockType fg = map.Tiles[x, y, 1].Block;
                        BlockType bg = map.Tiles[x, y, 0].Block;
                        SaveFlags sf = SaveFlags.None;

                        //Calculate Run-Length Encoding
                        int i = 0;
                        while (i + 1 + x < map.Width && fg.ID == map.Tiles[x + i + 1, y, 1].Block.ID && bg.ID == map.Tiles[x + i + 1, y, 0].Block.ID)
                            i++; //If next block is the same, record the amount of same blocks in i


                        //Calculate flags
                        if (i > 0)
                            sf |= SaveFlags.RLE;
                        if (fg.ID != BlockType.Empty.ID)
                            sf |= SaveFlags.Foreground;
                        if (bg.ID != BlockType.Empty.ID)
                            sf |= SaveFlags.Background;
                        binaryWriter.Write((byte)sf);

                        //Save data
                        if (i > 0)
                            binaryWriter.Write((short)i);
                        if (fg.ID != BlockType.Empty.ID)
                            binaryWriter.Write(fg.ID);
                        if (bg.ID != BlockType.Empty.ID)
                            binaryWriter.Write(bg.ID);

                        x += i;
                    }
                }
            }
        }

        [Flags]
        public enum SaveFlags : byte
        {
            None = 1, //Tile contains no flags
            Foreground = 2,
            Background = 4, //Tile contains background
            RLE = 8, //Run length encoding
        }
        #endregion
    }
}
