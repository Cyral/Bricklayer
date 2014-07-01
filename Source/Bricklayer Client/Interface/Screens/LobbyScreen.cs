using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    public class LobbyScreen : BaseScreen
    {
        //Controls
        public LobbyWindow Lobby;

        public string Description;
        public string Name;
        public string Intro;
        public int Online;
        public List<Bricklayer.Common.Data.LobbySaveData> Rooms;

        public override void Add(ScreenManager screenManager)
        {
            Game.CurrentGameState = GameState.Lobby;
            base.Add(screenManager);
            (Manager.Game as Application).BackgroundImage = ContentPack.Textures["gui\\background"];
            //Add the login window
            Lobby = new LobbyWindow(Manager);
            Lobby.Init();
            Window.Add(Lobby);
            Lobby.Show();
        }
        public override void Remove()
        {
            Window.Remove(Lobby);
            (Manager.Game as Application).BackgroundImage = null;
        }
    }
}
