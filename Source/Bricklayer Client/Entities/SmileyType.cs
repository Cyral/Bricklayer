﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BricklayerClient.Entities
{
    /// <summary>
    /// Smiley Type
    /// </summary>
    public class SmileyType
    {
        /// <summary>
        /// List of all block types
        /// </summary>
        public static List<SmileyType> SmileyList;
        public static SmileyType Default { get { return SmileyType.Smile; }}
        /// <summary>
        /// Name of the smiley
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// ID used for loading and saving
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
        /// <summary>
        /// Texture
        /// </summary>
        public Texture2D Texture { get; set; }

        public static SmileyType Smile, Happy, Laughing, Tounge, Oh, Meh, Sad, Upset;

        /// <summary>
        /// Creates a new instance of a blocktype
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <param name="index">The Y position of the smiley in the spritesheet</param>
        public SmileyType(string name, int index)
        {
            Name = name;
            RightSource = new Rectangle(0, index, Player.WIDTH, Player.HEIGHT);
            LeftSource = new Rectangle(Player.WIDTH, index, Player.WIDTH, Player.HEIGHT);
            ID = (byte)SmileyList.Count();
            SmileyList.Add(this);
        }
        static SmileyType()
        {
            SmileyList = new List<SmileyType>();
            Init();
        }
        /// <summary>
        /// Add all of the achivement configs here
        /// </summary>
        public static void Init()
        {
            Smile = new SmileyType("Smile", 0);
            Happy = new SmileyType("Happy", Player.HEIGHT);
            Laughing = new SmileyType("Laughing", Player.HEIGHT * 2);
            Tounge = new SmileyType("Tounge", Player.HEIGHT * 3);
            Oh = new SmileyType("Oh", Player.HEIGHT * 4);
            Meh = new SmileyType("Meh", Player.HEIGHT * 5);
            Sad = new SmileyType("Sad", Player.HEIGHT * 6);
            Upset = new SmileyType("Upset", Player.HEIGHT * 7);
        }
        public static SmileyType FromID(byte ID)
        {
            return SmileyList.First(x => x.ID == ID);
        }
    }
}