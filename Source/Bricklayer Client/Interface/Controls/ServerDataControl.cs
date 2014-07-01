using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Bricklayer.Client.Networking;
using TomShane.Neoforce.Controls;
using Bricklayer.Common.Data;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// A control for displaying a servers name, players online, motd, etc in the serverlist
    /// </summary>
    public class ServerDataControl : Control
    {
        public ServerSaveData Data;
        public ServerPingData Ping;

        private Label Name, Motd, Stats, Host;
        private StatusBar Gradient;

        public ServerDataControl(Manager manager, ServerPinger pinger, ServerSaveData server)
            : base(manager)
        {
            //Setup
            Passive = false;
            Height = 76;
            ClientWidth = 450 - 16;
            Data = server;

            //Background "gradient" image
            //TODO: Make an actual control. not a statusbar
            Gradient = new StatusBar(manager);
            Gradient.Init();
            Gradient.Alpha = .8f;
            Add(Gradient);

            //Add controls
            Name = new Label(Manager) { Width = this.Width, Text = server.Name, Left = 4, Top = 4, Font = FontSize.Default14, Alignment = Alignment.TopLeft};
            Name.Init();
            Add(Name);

            Stats = new Label(Manager) { Width = this.Width, Text = string.Empty, Alignment = Alignment.TopLeft, Top = 4, Font = FontSize.Default14, };
            Stats.Init();
            Add(Stats);

            Motd = new Label(Manager) { Width = this.Width, Left = 4, Top = Name.Bottom + 8, Font = FontSize.Default8, Alignment = Alignment.TopLeft };
            Motd.Init();
            Motd.Text = "Querying server for data...";
            Motd.Height = (int)Manager.Skin.Fonts["Default8"].Height * 2;
            Add(Motd);

            Host = new Label(Manager) { Width = this.Width, Text = server.GetHostString(), Alignment = Alignment.TopLeft, Left = 4, Top = Motd.Bottom, TextColor = Color.LightGray };
            Host.Init();
            Add(Host);

            //Ping the server for stats on a seperate thread
            new Thread(() => PingServer(pinger)).Start();
        }
        private void PingServer(ServerPinger pinger)
        {
            string error = "";
            ServerPingData ping = pinger.PingServer(Data.IP, Data.Port, out error);
            Ping = ping;
            //If no error
            if (error == string.Empty)
            {
                //Set the controls with the recieved data
                Stats.Text = ping.Online + "/" + ping.MaxOnline;
                Stats.Left = (ClientWidth - (int)Manager.Skin.Fonts["Default14"].Resource.MeasureString(Stats.Text).X) - 4 - 32;
                Motd.Text = ping.Description;
            }
            else
            {
                Motd.Text = error;
                Motd.TextColor = Color.Gold;
            }
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            //Don't draw anything
            //base.DrawControl(renderer,rect,gameTime);
        }
    }
}
