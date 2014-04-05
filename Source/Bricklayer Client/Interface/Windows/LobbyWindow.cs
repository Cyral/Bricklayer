using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bricklayer.Client.Networking;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// A small window containing the lobby room list and server data
    /// </summary>
    public class LobbyWindow : Dialog
    {
        public LobbyWindow(Manager manager)
            : base(manager)
        {
            //Setup the window
            CaptionVisible = false;
            TopPanel.Visible = false;
            Movable = false;
            Resizable = false;
            Width = 700;
            Height = 500;
            Shadow = true;
            Center();
            
        }
    }
}
