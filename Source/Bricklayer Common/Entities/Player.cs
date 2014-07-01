using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bricklayer.Common.World;
using Microsoft.Xna.Framework;

namespace Bricklayer.Common.Entities
{
    public abstract class Player
    {
        //Contants
        public const int Width = 16, Height = 16;

        //Entity States
        public EntityState SimulationState; //Current internal state (What the game sees)
        public EntityState DisplayState; //Current display state (What the player sees)
        public EntityState PreviousState; //Last internal state
        public PlayerMode Mode; //Godmode or regular?
        public FacingDirection Direction;
        public GravityDirection GravityDirection;
        public GravityDirection JumpDirection;

        //Data
        public virtual byte ID { get; set; }
        public virtual int Index { get; set; }
        public virtual string Username { get; set; }

        //Physic States
        public bool IsJumping { get; set; }
        public bool IsOnGround { get; set; }
        public bool WasJumping { get; set; }
        public float JumpTime { get; set; }
        public float IdleTime { get; set; }
        public bool IsIdle { get { return IdleTime > 0; } }
        public bool VirtualJump { get; set; }

        /// <summary>
        /// The currently occupied map the player is in
        /// </summary>
        public virtual Map Map { get; set; }

        /// <summary>
        /// The current smiley the player is using
        /// </summary>
        public virtual SmileyType Smiley { get; set; }

        /// <summary>
        /// The color the body should be tinted
        /// </summary>
        public virtual Color Tint { get; set; } //Hue-Hue :3

        /// <summary>
        /// The rectangular collision bounds of the player
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)SimulationState.Position.X, (int)SimulationState.Position.Y, Width, Height);
            }
        }

        /// <summary>
        /// The position on the grid a player is occupying (based of DisplayState)
        /// </summary>
        public virtual Point GridPosition { get { return new Point((int)Math.Round(DisplayState.Position.X / Tile.Width), (int)Math.Round(DisplayState.Position.Y / Tile.Height)); } }

        /// <summary>
        /// Creates a new base of a player
        /// </summary>
        public Player(Map map, Vector2 position, string name, int id)
        {
            Map = map;
            Smiley = SmileyType.Default;
            Mode = PlayerMode.Normal;
            Tint = Color.White;

            SimulationState = new EntityState();
            DisplayState = new EntityState();
            PreviousState = new EntityState();

            SimulationState.Position = PreviousState.Position = DisplayState.Position = position;

            if (id > byte.MaxValue)
                throw new ArgumentOutOfRangeException("id", id, "The player ID must be within range of a single byte.");
            ID = (byte)id;
            Username = name;

            Index = map.Players.Count;
        }
    }
}
