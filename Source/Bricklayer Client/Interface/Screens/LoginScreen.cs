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
    public class LoginScreen : BaseScreen
    {
        //Config
        private string githubLink = "https://github.com/Cyral/Bricklayer";
        //Controls
        public ImageBox LogoImage, GithubIcon;
        public Label Version;
        public ServerWindow Login;

        public override void Add(ScreenManager screenManager)
        {
            base.Add(screenManager);
            (Manager.Game as Application).BackgroundImage = ContentPack.Textures["gui\\background"];

            //Add the logo image
            LogoImage = new ImageBox(Manager) { Image = ContentPack.Textures["gui\\logosmall"], SizeMode = SizeMode.Centered };
            LogoImage.SetSize(LogoImage.Image.Width, LogoImage.Image.Height);
            LogoImage.SetPosition((Window.Width / 2) - (LogoImage.Width / 2), 0);
            LogoImage.Init();
            Window.Add(LogoImage);

            //Add github contribute link
            GithubIcon = new ImageBox(Manager) { Image = ContentPack.Textures["gui\\github"], SizeMode = SizeMode.Auto, };
            GithubIcon.ToolTip.Text = "We love open source! Contribute to Bricklayer at our GitHub repo.";
            GithubIcon.SetSize(GithubIcon.Width, GithubIcon.Height);
            GithubIcon.SetPosition(Window.Width - GithubIcon.Width - 8, Window.Height - GithubIcon.Height - 8);
            GithubIcon.Init();
            GithubIcon.Color = Color.White * .6f;
            //Click/Hover events
            GithubIcon.MouseOut += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                GithubIcon.Color = Color.White * .6f;
            });
            GithubIcon.MouseOver += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                GithubIcon.Color = Color.White;
            });
            GithubIcon.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (Manager.Game.IsActive) Process.Start(githubLink); //Open the link in a browser
            });
            Window.Add(GithubIcon);

            //Add version tag
            Version = new Label(Manager) { Font = FontSize.Default14 };
            Version.SetSize(200, 16);
            Version.SetPosition(8, Window.Height - Version.Height - 8);
            Version.Init();
            Version.Text = AssemblyVersionName.GetVersion();
            Window.Add(Version);

            //Add the login window
            Login = new ServerWindow(Manager);
            Login.Init();
            if (Login.Top - 24 < LogoImage.Height + LogoImage.Top) //If it is too close to logo, move it down a bit
                Login.Top = LogoImage.Height + LogoImage.Top - 24;
            Window.Add(Login);
            Login.Show();
        }
        public override void Remove()
        {
            Window.Remove(LogoImage);
            Window.Remove(GithubIcon);
            Window.Remove(Version);
            Window.Remove(Login);
            (Manager.Game as Application).BackgroundImage = null;
        }
    }
}
