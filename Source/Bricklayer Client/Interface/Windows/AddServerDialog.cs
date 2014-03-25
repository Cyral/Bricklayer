using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BricklayerClient.Networking;
using TomShane.Neoforce.Controls;

namespace BricklayerClient.Interface
{
    /// <summary>
    /// Dialog for adding/editing servers on the server list
    /// </summary>
    public class AddServerDialog : Dialog
    {
        private bool Edit; //Edit OR Add a server?
        private int Index; //Index of the server in the list

        //Controls
        private Button SaveBtn;
        private TextBox NameTxt, AddressTxt;
        private Label NameLbl, AddressLbl;
        private ServerWindow ServerList;

        public AddServerDialog(Manager manager, ServerWindow parent, int index, bool edit, string name, string address)
            : base(manager)
        {
            //Are we editing a server or adding one (They use same dialog)
            Edit = edit;
            Index = index;
            ServerList = parent;
            //Setup the window
            Text = Edit ? "Edit Server" : "Add Server";
            TopPanel.Visible = false;
            Resizable = false;
            Width = 250;
            Height = 180;
            Center();

            //Add controls
            NameLbl = new Label(manager) { Left = 8, Top = 8, Text = "Name:", Width = this.ClientWidth - 16 };
            NameLbl.Init();
            Add(NameLbl);

            NameTxt = new TextBox(manager) { Left = 8, Top = NameLbl.Bottom + 4, Width = this.ClientWidth - 16 };
            NameTxt.Init();
            NameTxt.Text = name;
            Add(NameTxt);

            AddressLbl = new Label(manager) { Left = 8, Top = NameTxt.Bottom + 8, Text = "Address: (Default port is 14242)", Width = this.ClientWidth - 16 };
            AddressLbl.Init();
            Add(AddressLbl);

            AddressTxt = new TextBox(manager) { Left = 8, Top = AddressLbl.Bottom + 4, Width = this.ClientWidth - 16 };
            AddressTxt.Init();
            AddressTxt.Text = address;
            Add(AddressTxt);

            SaveBtn = new Button(manager) { Top = 8, Text = Edit ? "Save" : "Add", };
            SaveBtn.Init();
            SaveBtn.Left = (Width / 2) - (SaveBtn.Width / 2);
            SaveBtn.Click += SaveBtn_Click;
            BottomPanel.Add(SaveBtn);
        }
        /// <summary>
        /// When the saved button is clicked
        /// </summary>
        void SaveBtn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (Edit)
            {
                //TODO: Add validation and error handling
                string[] address = AddressTxt.Text.Split(':');
                string IP;
                int port = 0;
                IP = address[0];
                if (address.Length > 1)
                    port = int.Parse(address[1]);
                ServerList.EditServer(Index, new ServerSaveData(NameTxt.Text, IP, port));
            }
            else
            {
                //TODO: Add validation and error handling
                string[] address = AddressTxt.Text.Split(':');
                string IP;
                int port = 0;
                IP = address[0];
                if (address.Length > 1)
                    port = int.Parse(address[1]);
                ServerList.AddServer(new ServerSaveData(NameTxt.Text, IP, port));
            }
            Close();
        }
    }
}
