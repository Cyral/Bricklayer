using System.Windows.Forms;

namespace Bricklayer.Common
{
    partial class ExceptionForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionForm));
            this.lblInfo = new System.Windows.Forms.Label();
            this.errorBox = new System.Windows.Forms.TextBox();
            this.btnForums = new System.Windows.Forms.Button();
            this.lblMain = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInfo.Location = new System.Drawing.Point(14, 48);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(420, 39);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = resources.GetString("lblInfo.Text");
            // 
            // errorBox
            // 
            this.errorBox.BackColor = System.Drawing.SystemColors.Window;
            this.errorBox.HideSelection = false;
            this.errorBox.Location = new System.Drawing.Point(12, 98);
            this.errorBox.Multiline = true;
            this.errorBox.Name = "errorBox";
            this.errorBox.ReadOnly = true;
            this.errorBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorBox.Size = new System.Drawing.Size(646, 248);
            this.errorBox.TabIndex = 1;
            // 
            // btnForums
            // 
            this.btnForums.ForeColor = System.Drawing.Color.Black;
            this.btnForums.Location = new System.Drawing.Point(519, 8);
            this.btnForums.Name = "btnForums";
            this.btnForums.Size = new System.Drawing.Size(139, 54);
            this.btnForums.TabIndex = 4;
            this.btnForums.Text = "Report issue\r\non GitHub";
            this.btnForums.UseVisualStyleBackColor = true;
            this.btnForums.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.Location = new System.Drawing.Point(13, 8);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(386, 40);
            this.lblMain.TabIndex = 7;
            this.lblMain.Text = "Bricklayer has encountered an error and has crashed!\r\nWe are sorry for the inconv" +
    "enience. ";
            // 
            // btnCopy
            // 
            this.btnCopy.ForeColor = System.Drawing.Color.Blue;
            this.btnCopy.Location = new System.Drawing.Point(519, 68);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(139, 24);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "Copy Text";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // ExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 358);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.btnForums);
            this.Controls.Add(this.errorBox);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bricklayer Error";
            this.Load += new System.EventHandler(this.ExceptionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TextBox errorBox;
        private Button btnForums;
        private Label lblMain;
        private Button btnCopy;
    }
}

