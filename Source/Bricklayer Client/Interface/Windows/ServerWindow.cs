using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bricklayer.Client.Networking;
using TomShane.Neoforce.Controls;
using Cyral.Extensions;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// A small window containing the server list and editing controls
    /// </summary>
    public class ServerWindow : Dialog
    {
        public ServerPinger Pinger;
        public ControlList<ServerDataControl> ServerListCtrl;
        public List<ServerSaveData> Servers;

        private Button JoinBtn, AddBtn, RemoveBtn, EditBtn, RefreshBtn;
        private TextBox NameTxt;
        private Label NameLbl, ColorLbl;
        private ImageBox BodyImg, SmileyImg;
        private ColorPicker BodyClr;


        public ServerWindow(Manager manager) : base(manager)
        {
            //Setup server pinger/query
            Pinger = new ServerPinger();
            //Setup the window
            CaptionVisible = false;
            Caption.Text = "Welcome to Bricklayer!";
            Description.Text = "An open source, fully moddable and customizable 2D\nbuilding game built with the community in mind.";
            Movable = false;
            Resizable = false;
            Width = 450;
            Height = 350;
            Shadow = true;
            Center();

            //Player config
            NameLbl = new Label(Manager) { Left = 8, Top = TopPanel.Bottom + 10, Width = 64, Text = "Username:" };
            NameLbl.Init();
            Add(NameLbl);

            NameTxt = new TextBox(Manager) { Left = NameLbl.Right + 4, Top = TopPanel.Bottom + 8, Width = 150, Text = Game.Username};
            NameTxt.Init();
            NameTxt.Refresh();
            NameTxt.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (NameTxt.Text.Length > Settings.MaxNameLength) //Clamp length
                    NameTxt.Text = NameTxt.Text.Truncate(Settings.MaxNameLength);
                Game.Username = NameTxt.Text;
            });
            Add(NameTxt);

            ColorLbl = new Label(Manager) { Left = NameTxt.Right + 8, Top = TopPanel.Bottom + 10, Width = 36, Text = "Color:" };
            ColorLbl.Init();
            Add(ColorLbl);

            BodyClr = new ColorPicker(Manager) { Left = ColorLbl.Right + 4, Top = TopPanel.Bottom + 8, Width = 128, Saturation = IO.ColorSaturation, Value = IO.ColorValue };
            BodyClr.Init();
            BodyClr.ValueChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                BodyImg.Color = BodyClr.SelectedColor;
                Game.MyColor = BodyClr.SelectedColor;
                Game.MyHue = BodyClr.Hue;
            });
            Add(BodyClr);

            BodyImg = new ImageBox(Manager) { Left = BodyClr.Right + 6, Top = TopPanel.Bottom + 8, Width = 18, Height = 18, Color = Game.MyColor, Image = ContentPack.Textures["entity\\body"] };
            BodyImg.Init();
            Add(BodyImg);

            SmileyImg = new ImageBox(Manager) { Left = BodyClr.Right + 6, Top = TopPanel.Bottom + 8, Width = 18, Height = 18, Image = ContentPack.Textures["entity\\smileys"], SourceRect = new Rectangle(0, 0, 18, 18) };
            SmileyImg.Init();
            SmileyImg.ToolTip.Text = "I love this color!";
            Add(SmileyImg);

            BodyClr.Hue = Game.MyHue;
            //Create main server list
            ServerListCtrl = new ControlList<ServerDataControl>(manager) { Left = 8, Top = TopPanel.Bottom + 34, Width = ClientWidth - 16, Height = ClientHeight - TopPanel.Height - BottomPanel.Height - 42 };
            ServerListCtrl.Init();
            Add(ServerListCtrl);
            RefreshServerList();

            //Add BottomPanel controls
            JoinBtn = new Button(manager) { Text = "Connect", Left = 24, Top = 8, Width = 100, };
            JoinBtn.Init();
            JoinBtn.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                //Logical place to save name/color settings
                IO.SaveSettings(new Settings() { Username = Game.Username, Color = Game.MyHue, ContentPack = Game.ContentPackName, Resolution = new Point(Game.Resolution.Width, Game.Resolution.Height), UseVSync = Game.MainWindow.Manager.Graphics.SynchronizeWithVerticalRetrace });
                //Connect
                if (ServerListCtrl.Items.Count > 0)
                {
                    if ((ServerListCtrl.Items[ServerListCtrl.ItemIndex] as ServerDataControl).Ping != null && (ServerListCtrl.Items[ServerListCtrl.ItemIndex] as ServerDataControl).Ping.Error)
                        return;
                    //Create a world and connect
                    JoinBtn.Enabled = false;
                    JoinBtn.Text = "Connecting...";
                    Game.NetManager.Connect(Servers[ServerListCtrl.ItemIndex].IP, Servers[ServerListCtrl.ItemIndex].Port);
                }
            });
            BottomPanel.Add(JoinBtn);

            AddBtn = new Button(manager) { Text = "Add", Left = JoinBtn.Right + 8, Top = 8, Width = 64, };
            AddBtn.Init();
            AddBtn.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                AddServerDialog window = new AddServerDialog(manager, this,ServerListCtrl.ItemIndex, false, string.Empty, string.Empty);
                window.Init();
                Manager.Add(window);
                window.Show();
            });
            BottomPanel.Add(AddBtn);

            EditBtn = new Button(manager) { Text = "Edit", Left = AddBtn.Right + 8, Top = 8, Width = 64, };
            EditBtn.Init();
            EditBtn.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (ServerListCtrl.Items.Count > 0)
                {
                    AddServerDialog window = new AddServerDialog(manager, this, ServerListCtrl.ItemIndex, true, Servers[ServerListCtrl.ItemIndex].Name, Servers[ServerListCtrl.ItemIndex].GetHostString());
                    window.Init();
                    Manager.Add(window);
                    window.Show();
                }
            });
            BottomPanel.Add(EditBtn);

            RemoveBtn = new Button(manager) { Text = "Remove", Left = EditBtn.Right + 8, Top = 8, Width = 64 };
            RemoveBtn.Init();
            RemoveBtn.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (ServerListCtrl.Items.Count > 0)
                {
                    MessageBox confirm = new MessageBox(manager, MessageBoxType.YesNo, "Are you sure you would like to remove\nthis server from your server list?", "[color:Red]Confirm Removal[/color]");
                    confirm.Init();
                    confirm.Closed += new WindowClosedEventHandler(delegate(object sender, WindowClosedEventArgs args)
                    {
                        if ((sender as Dialog).ModalResult == ModalResult.Yes) //If user clicked yes
                        {
                            Servers.RemoveAt(ServerListCtrl.ItemIndex);
                            ServerListCtrl.Items.RemoveAt(ServerListCtrl.ItemIndex);
                            IO.WriteServers(Servers);
                            RefreshServerList();
                        }
                    });
                    Manager.Add(confirm);
                    confirm.Show();
                }
            });
            BottomPanel.Add(RemoveBtn);

            RefreshBtn = new Button(manager) { Text = "Refresh", Left = RemoveBtn.Right + 8, Top = 8, Width = 64 };
            RefreshBtn.Init();
            RefreshBtn.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                RefreshServerList();
            });
            BottomPanel.Add(RefreshBtn);
            MainWindow.ScreenManager.FadeIn();
        }
        public void AddServer(ServerSaveData server)
        {
            Servers.Add(server);
            IO.WriteServers(Servers);
            RefreshServerList();
        }
        public void EditServer(int index,ServerSaveData server)
        {
            Servers[index] = server;
            IO.WriteServers(Servers);
            RefreshServerList();
        }
        private void RefreshServerList()
        {
            ServerListCtrl.Items.Clear();
            //Read the servers from config
            Servers = IO.ReadServers();
            //Populate the list 
            for (int i = 0; i < Servers.Count; i++)
                ServerListCtrl.Items.Add(new ServerDataControl(Manager, Pinger, Servers[i]));
            if (ServerListCtrl.Items.Count > 0)
                ServerListCtrl.ItemIndex = 0;
        }

        internal void Disconnected()
        {
            //Re-enable join button and display error if couldnt connect
            if (!JoinBtn.Enabled)
            {
                JoinBtn.Enabled = true;
                JoinBtn.Text = "Connect";
                MessageBox error = new MessageBox(Manager, MessageBoxType.Error, "Could not connect to server.", "[color:Gold]Error[/color]");
                error.Init();
                Manager.Add(error);
                error.Show();
            }
        }
    }
}
