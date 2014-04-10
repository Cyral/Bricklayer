using System;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bricklayer.Client.Entities;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// Handles the logical updating of UI elements
    /// </summary>
    public partial class MainWindow : Window
    {
        public int TotalFrames { get; set; }
        public int FPS { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        protected override void Update(GameTime gameTime)
        {
            switch (Game.CurrentGameState)
            {
                case GameState.Login:
                    {
                        GameScreen screen = (ScreenManager.Current as GameScreen);
                        break;
                    }
                case GameState.Game:
                    {
                        GameScreen screen = (ScreenManager.Current as GameScreen);
                        //Fps Counter
                        ElapsedTime += gameTime.ElapsedGameTime;

                        if (ElapsedTime > TimeSpan.FromSeconds(1))
                        {
                            ElapsedTime -= TimeSpan.FromSeconds(1);
                            FPS = TotalFrames;
                            TotalFrames = 0;
                        }
                        //Set Statusbar
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Status: " + Game.NetManager.GetConnectionStatus().ToString());
                        sb.Append("    Ping: " + NetTime.ToReadable(Game.NetManager.Client.ServerConnection.AverageRoundtripTime));
                        sb.Append("    FPS: " + FPS);
                        sb.Append("    Players: " + Game.Map.Players.Count);
                        sb.Append("    Block: " + Game.Map.SelectedBlock.Name);
                        screen.StatsLabel.Text = sb.ToString();

                        //Handles opening and closing of the textbox
                        if (Keys.T.IsKeyToggled(Game.KeyState, Game.LastKeyState) && !screen.ChatBox.TextBox.Focused)
                        {
                            //If already focused, unfocus, else, focus the textbox
                            if (Manager.FocusedControl == screen.ChatBox.TextBox) 
                            {
                                screen.ChatBox.TextBox.Focused = false;
                                Manager.FocusedControl = this;
                            }
                            else
                                Manager.FocusedControl = screen.ChatBox.TextBox;
                        }
                        //Handles pressing tab in the textbox to autocomplete names
                        if (Keys.Tab.IsKeyToggled(Game.KeyState, Game.LastKeyState) && screen.ChatBox.TextBox.Focused)
                        {
                            string[] words = screen.ChatBox.TextBox.Text.Split(' ');
                            if (words.Length > 0)
                            {
                                //Find the last part of a word we typed
                                string lastWord = words[words.Length - 1];
                                int index = screen.ChatBox.TextBox.Text.Length - lastWord.Length;
                                //See if any players start with it (And if we already typed the username, don't worry about it)
                                Player player = Game.Map.Players.FirstOrDefault(p => p.Username.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith(lastWord.ToUpper(System.Globalization.CultureInfo.InvariantCulture)) && p.Username != lastWord);
                                if (player != null && player != Game.Me)
                                {
                                    //If so, set the textbox to autocomplete it, and set the caret to the end of the word
                                    screen.ChatBox.TextBox.Text = screen.ChatBox.TextBox.Text.Substring(0, index) + player.Username;
                                    screen.ChatBox.TextBox.CursorPosition = index + player.Username.Length;
                                }
                            }
                        }
                        break;
                    }
            }
            ScreenManager.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
