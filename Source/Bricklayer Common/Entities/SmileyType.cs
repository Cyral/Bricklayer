using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bricklayer.Common.Entities
{
    /// <summary>
    /// A type of expression that players will show
    /// </summary>
    public class SmileyType
    {
        /// <summary>
        /// List of all block types
        /// </summary>
        public static List<SmileyType> SmileyList;

        /// <summary>
        /// The default smiley to be used if no other is specified
        /// </summary>
        public static SmileyType Default { get { return SmileyType.Smile; }}

        /// <summary>
        /// Name of the smiley
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ID used for loading, saving, and networking
        /// </summary>
        public byte ID { get; private set; }

        /// <summary>
        /// The source rectangle the smiley is found in the sprite sheet when the player is flipped right
        /// </summary>
        public Rectangle RightSource { get; private set; }

        /// <summary>
        /// The source rectangle the smiley is found in the sprite sheet when the player is flipped left
        /// </summary>
        public Rectangle LeftSource { get; private set; }

        public static SmileyType Smile, Happy, Laughing, Tounge, Oh, Meh, Sad, Upset;

        /// <summary>
        /// Creates a new instance of a blocktype
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <param name="index">The Y position of the smiley in the spritesheet</param>
        public SmileyType(string name, int index)
        {
            Name = name;
            RightSource = new Rectangle(0, index, Player.Width, Player.Height);
            LeftSource = new Rectangle(Player.Width, index, Player.Width, Player.Height);
            ID = (byte)SmileyList.Count();
            SmileyList.Add(this);
        }

        static SmileyType()
        {
            SmileyList = new List<SmileyType>();
            Init();
        }

        /// <summary>
        /// Adds/creates all <c>SmileyType</c>s
        /// </summary>
        public static void Init()
        {
            Smile = new SmileyType("Smile", 0);
            Happy = new SmileyType("Happy", Player.Height);
            Laughing = new SmileyType("Laughing", Player.Height * 2);
            Tounge = new SmileyType("Tounge", Player.Height * 3);
            Oh = new SmileyType("Oh", Player.Height * 4);
            Meh = new SmileyType("Meh", Player.Height * 5);
            Sad = new SmileyType("Sad", Player.Height * 6);
            Upset = new SmileyType("Upset", Player.Height * 7);
        }

        /// <summary>
        /// Finds a <c>SmileyType</c> from a given ID number
        /// </summary>
        public static SmileyType FromID(byte ID)
        {
            return SmileyList.First(x => x.ID == ID);
        }
    }
}
