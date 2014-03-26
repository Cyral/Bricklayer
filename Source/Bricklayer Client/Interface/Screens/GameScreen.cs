using System.Text;
using Cyral.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace Bricklayer.Client.Interface
{
    /// <summary>
    /// The main screen to be used for game functions
    /// </summary>
    public class GameScreen : BaseScreen
    {
        public const int SidebarSize = 225 + 2;
        public Button[] Inventory;
        public StatusBar Bar, Sidebar;
        public Label StatsLabel;
        public Console ChatBox;
        public ListBox PlayerList;

        public override void Add(ScreenManager screenManager)
        {
            base.Add(screenManager);

            Bar = new StatusBar(Manager) { Top = Window.Height - 24, Width = Window.Width };
            Bar.Init();
            Window.Add(Bar);
            StatsLabel = new Label(Manager) { Top = 4, Left = 8, Width = Window.Width - 16, Text = "" };
            StatsLabel.Init();
            Bar.Add(StatsLabel);

            Sidebar = new StatusBar(Manager);
            Sidebar.Init();
            Sidebar.SetSize(SidebarSize, (int)((Window.Height - Bar.Height)));
            Sidebar.SetPosition(Window.Width - Sidebar.Width, 0);
            Window.Add(Sidebar);

            PlayerList = new ListBox(Manager);
            PlayerList.Init();
            PlayerList.SetSize(SidebarSize, (int)((Window.Height - Bar.Height - 4) * .25f));
            PlayerList.SetPosition(1, 2);
            Sidebar.Add(PlayerList);

            ChatBox = new Console(Manager);
            Manager.Add(ChatBox);
            ChatBox.Init();
            ChatBox.SetSize(PlayerList.Width, (int)((Window.Height - Bar.Height - 4) * .75f));
            ChatBox.SetPosition(Sidebar.Left + 1, PlayerList.Bottom + 1);
            ChatBox.ChannelsVisible = false;
            ChatBox.MessageSent += new ConsoleMessageEventHandler(SentChat);
            ChatBox.Channels.Add(new ConsoleChannel(0, "Global", Color.White));
            // Select default channel
            ChatBox.SelectedChannel = 0;
            // Do we want to add timestamp or channel name at the start of every message?
            ChatBox.MessageFormat = ConsoleMessageFormats.None;
            ChatBox.TextBox.TextChanged += TextBox_TextChanged;

            //Hide them until we recieve the Init packet
            ChatBox.Visible = PlayerList.Visible = Sidebar.Visible = false;
        }

        #region Chat Helper Methods
        /// <summary>
        /// Called when the content of the chatbox's text is changed
        /// </summary>
        void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            //Trim to max length
            if (txtBox.Text.StartsWith(Game.Username + ": "))
                txtBox.Text = txtBox.Text.Substring((Game.Username + ": ").Length);
            if (txtBox.Text.Length > Networking.Messages.ChatMessage.MaxLength)
                txtBox.Text = txtBox.Text.Truncate(Networking.Messages.ChatMessage.MaxLength);
        }
        /// <summary>
        /// Adds a system chat message to the chat box
        /// </summary>
        public void SystemChat(string message)
        {
            message = WrapText(ChatBox.GetFont(), "[color:Gold]*[/color]" + message, ChatBox.Width - 24);
            ConsoleMessage c = new ConsoleMessage(message, 0);
            string[] texts = c.Text.Split(new string[1] { TomShane.Neoforce.Controls.Manager.StringNewline }, System.StringSplitOptions.None);
            ConsoleMessage[] msgs = new ConsoleMessage[texts.Length];

            AddText(c, texts, msgs);
        }
        /// <summary>
        /// Adds a chat message to the chat box
        /// </summary>
        public void AddChat(string username, string message)
        {
            message = WrapText(ChatBox.GetFont(), username + ": " + message, ChatBox.Width - 24);
            ConsoleMessage c = new ConsoleMessage(message, 0);
            string[] texts = c.Text.Split(new string[1] { TomShane.Neoforce.Controls.Manager.StringNewline }, System.StringSplitOptions.None);
            ConsoleMessage[] msgs = new ConsoleMessage[texts.Length];

            AddText(c, texts, msgs);
        }
        /// <summary>
        /// Called when a chat is sent from the local user
        /// </summary>
        void SentChat(object sender, ConsoleMessageEventArgs e)
        {
            e.Message.Text = WrapText(ChatBox.GetFont(), Game.Username + ": " + e.Message.Text, ChatBox.Width - 24);
            ConsoleMessage c = new ConsoleMessage(e.Message.Text, 0);
            Game.NetManager.SendMessage(new Networking.Messages.ChatMessage(Game.Me, e.Message.Text.Substring((Game.Username + ": ").Length)));
            string[] texts = c.Text.Split(new string[1] { TomShane.Neoforce.Controls.Manager.StringNewline }, System.StringSplitOptions.None);
            ConsoleMessage[] msgs = new ConsoleMessage[texts.Length];

            AddText(c, texts, msgs);

            ChatBox.TextBox.Focused = false;
            Manager.FocusedControl = Game.MainWindow;
        }
        /// <summary>
        /// Used to add text messages to the chat box
        /// </summary>
        private void AddText(ConsoleMessage c, string[] texts, ConsoleMessage[] msgs)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                string str = texts[i];
                msgs[i] = new ConsoleMessage(str, c.Channel, c.Color);
                msgs[i].NoShow = i > 0;
                if (!string.IsNullOrWhiteSpace(msgs[i].Text))
                    ChatBox.MessageBuffer.Add(msgs[i]);
            }
        }
        /// <summary>
        /// Wraps a string around the width of an area (Ex: The chat box)
        /// </summary>
        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureRichString(word, Manager);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append(TomShane.Neoforce.Controls.Manager.StringNewline + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Called when we recieve the Init packet, meaning we can now show game UI and hide the loading message
        /// </summary>
        public void Show()
        {
            ChatBox.Visible = PlayerList.Visible = Sidebar.Visible = true;
        }
        public override void Remove()
        {
            //TODO
        }

    }
}
