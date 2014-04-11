﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Client.Networking
{
    /// <summary>
    /// Room data for displaying in the lobby list, to be serialized to JSON
    /// </summary>
    public class LobbySaveData
    {
        public string Name;
        public string Description;
        public int Online;
        public double Rating;
        public int Plays;
        public int ID;

        public LobbySaveData(string name, int id, string description, int players, int plays, double rating)
        {
            Name = name;
            ID = id;
            Description = description;
            Online = players;
            Rating = rating;
            Plays = plays;
        }
        /// <summary>
        /// Creates a LobbySaveData instance from a Map
        /// </summary>
        public static LobbySaveData FromMap(World.Map map)
        {
            return new LobbySaveData(map.Name, map.ID, map.Description, map.Online, map.Plays, map.Rating);
        }
    }
}
