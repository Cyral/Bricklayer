using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Cyral.Extensions;
using Microsoft.Xna.Framework;

namespace BricklayerClient
{
    public class ContentPack
    {
        /// <summary>
        /// The name of the content pack
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A short description of the content pack
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Version of the content pack, defined by creator, no set guidelines
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Author(s)
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Absolute path to the content pack on disk
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Defines if it is embedded with the application, in the content folder, not in the .Zarknorth folder
        /// </summary>
        public bool Embedded { get; set; }
        /// <summary>
        /// The icon to be shown when selecting this pack
        /// </summary>
        public Texture2D Icon { get; set; }

        /// <summary>
        /// List of all textures loaded into the game, according to the pack
        /// </summary>
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Loads a pack into the game
        /// </summary>
        /// <param name="pack">ContentPack to load</param>
        public static void LoadPack(ContentPack pack)
        {
            //Clear all memory in case we are reloading
            Textures.Clear();
            if (!pack.Embedded) //If extenal pack, first load default
                LoadTextures(IO.ContentPacks[0]);
            LoadTextures(pack);
        }
        /// <summary>
        /// Load textures for a given pack
        /// </summary>
        private static void LoadTextures(ContentPack pack)
        {
            //List of texture file names
            List<string> Files = null;
            //Base directory for all textures
            string BaseDirectory;
            Texture2D Texture = null;

            //Get the files name list for the pack, and the directory to it for loading
            if (pack.Embedded)
            {
                Files = DirSearch(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Content\\Content Packs\\" + pack.Name, System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Content\\Content Packs\\" + pack.Name + "\\textures", ".xnb");
                BaseDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Content\\Content Packs\\" + pack.Name + "\\textures\\";
            }
            else
            {
                Files = DirSearch(IO.Directories["Content Packs"] + Game.ContentPackName, IO.Directories["Content Packs"] + Game.ContentPackName + "\\textures", ".png");
                BaseDirectory = IO.Directories["Content Packs"] + Game.ContentPackName + "\\textures\\";
            }

            //For each file name, load it from disk;
            for (int i = 0; i < Files.Count; i++)
            {
                //Remove the full path to return the name of the file
                string name = GetName(Files[i], BaseDirectory, i);

                if (pack.Embedded)
                    Texture = Game.TextureLoader.Content.Load<Texture2D>("Content\\Content Packs\\Default\\textures\\" + name);
                else
                    Texture = Game.TextureLoader.FromFile(Files[i]);

                //Add it to the dictionary
                if (Textures.ContainsKey(name))
                    Textures[name] = Texture;
                else
                    Textures.Add(name, Texture);
            }
        }
        
        /// <summary>
        /// Get the name of a file by removing the base path from it
        /// </summary>
        private static string GetName(string FullPath, string BaseDirectory, int i)
        {
            string name = FullPath;
            name = name.Substring(BaseDirectory.Length);
            name = name.Substring(0, name.LastIndexOf('.'));
            return name;
        }
        /// <summary>
        /// Recursively search a directory for sub folders and files
        /// </summary>
        /// <returns>A list of files within the folder or subfolders</returns>
        private static List<String> DirSearch(string basedir, string directory, params string[] extensions)
        {
            List<String> files = new List<String>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            try
            {
                foreach (FileInfo f in dir.GetFiles("*.*"))
                {
                    if (f.Extension.EqualsAny(extensions))
                        files.Add(f.FullName);
                }
                foreach (DirectoryInfo d in dir.GetDirectories("*.*"))
                {
                    files.AddRange(DirSearch(basedir, d.FullName, extensions));
                }
            }
            catch (System.Exception excpt)
            {
                System.Windows.Forms.MessageBox.Show(excpt.Message);
            }

            return files;
        }
    }
}
