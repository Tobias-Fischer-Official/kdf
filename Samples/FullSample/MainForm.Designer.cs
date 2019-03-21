namespace KDFFullSample
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxPw = new System.Windows.Forms.TextBox();
            this.tbxChannel = new System.Windows.Forms.TextBox();
            this.tbxNick = new System.Windows.Forms.TextBox();
            this.tbxLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxProxyPort = new System.Windows.Forms.TextBox();
            this.tbxProxyIP = new System.Windows.Forms.TextBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.htmlChatHistory = new KDF.Graphics.HTMLChatHistory();
            this.userListControl = new KDF.Graphics.UserList.UserListControl();
            this.extendedUserInput = new KDF.Graphics.ExtendedInput();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 108);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Verbinden";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(95, 108);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "Einloggen";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nickname";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Passwort";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Channel";
            // 
            // tbxPw
            // 
            this.tbxPw.Location = new System.Drawing.Point(70, 38);
            this.tbxPw.Name = "tbxPw";
            this.tbxPw.PasswordChar = '*';
            this.tbxPw.Size = new System.Drawing.Size(100, 20);
            this.tbxPw.TabIndex = 6;
            this.tbxPw.Text = "Passwort";
            // 
            // tbxChannel
            // 
            this.tbxChannel.Location = new System.Drawing.Point(70, 64);
            this.tbxChannel.Name = "tbxChannel";
            this.tbxChannel.Size = new System.Drawing.Size(100, 20);
            this.tbxChannel.TabIndex = 7;
            this.tbxChannel.Text = "Channel";
            // 
            // tbxNick
            // 
            this.tbxNick.Location = new System.Drawing.Point(70, 12);
            this.tbxNick.Name = "tbxNick";
            this.tbxNick.Size = new System.Drawing.Size(100, 20);
            this.tbxNick.TabIndex = 8;
            this.tbxNick.Text = "Nickname";
            // 
            // tbxLog
            // 
            this.tbxLog.Location = new System.Drawing.Point(15, 153);
            this.tbxLog.Multiline = true;
            this.tbxLog.Name = "tbxLog";
            this.tbxLog.Size = new System.Drawing.Size(306, 349);
            this.tbxLog.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Log";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(265, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "ProxyIP";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(265, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "ProxyPort";
            // 
            // tbxProxyPort
            // 
            this.tbxProxyPort.Location = new System.Drawing.Point(323, 38);
            this.tbxProxyPort.Name = "tbxProxyPort";
            this.tbxProxyPort.Size = new System.Drawing.Size(100, 20);
            this.tbxProxyPort.TabIndex = 15;
            this.tbxProxyPort.Text = "9050";
            // 
            // tbxProxyIP
            // 
            this.tbxProxyIP.Location = new System.Drawing.Point(324, 12);
            this.tbxProxyIP.Name = "tbxProxyIP";
            this.tbxProxyIP.Size = new System.Drawing.Size(100, 20);
            this.tbxProxyIP.TabIndex = 16;
            this.tbxProxyIP.Text = "127.0.0.1";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(181, 108);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 17;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.userListControl);
            this.panel1.Location = new System.Drawing.Point(845, 153);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 375);
            this.panel1.TabIndex = 20;
            // 
            // htmlChatHistory
            // 
            this.htmlChatHistory.Location = new System.Drawing.Point(324, 153);
            this.htmlChatHistory.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlChatHistory.Name = "htmlChatHistory";
            this.htmlChatHistory.Size = new System.Drawing.Size(515, 349);
            this.htmlChatHistory.TabIndex = 21;
            // 
            // userListControl
            // 
            this.userListControl.AutoScroll = true;
            this.userListControl.AutoSize = true;
            this.userListControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.userListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userListControl.HeaderBackColor = System.Drawing.Color.Empty;
            this.userListControl.HeaderForeColor = System.Drawing.Color.Empty;
            this.userListControl.HeaderTitle = null;
            this.userListControl.ItemDescription = null;
            this.userListControl.ListName = null;
            this.userListControl.Location = new System.Drawing.Point(0, 0);
            this.userListControl.Name = "userListControl";
            this.userListControl.Size = new System.Drawing.Size(200, 375);
            this.userListControl.TabIndex = 20;
            // 
            // extendedUserInput
            // 
            this.extendedUserInput.AcceptsTab = true;
            this.extendedUserInput.AutoCompleteEntrys = ((System.Collections.Generic.List<string>)(resources.GetObject("extendedUserInput.AutoCompleteEntrys")));
            this.extendedUserInput.LastInputs = ((System.Collections.Generic.Dictionary<int, string>)(resources.GetObject("extendedUserInput.LastInputs")));
            this.extendedUserInput.Location = new System.Drawing.Point(324, 508);
            this.extendedUserInput.Multiline = true;
            this.extendedUserInput.Name = "extendedUserInput";
            this.extendedUserInput.Size = new System.Drawing.Size(515, 20);
            this.extendedUserInput.TabCacheEntrys = ((System.Collections.Generic.List<string>)(resources.GetObject("extendedUserInput.TabCacheEntrys")));
            this.extendedUserInput.TabIndex = 18;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(176, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 20);
            this.btnLoad.TabIndex = 22;
            this.btnLoad.Text = "Laden";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 85);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 20);
            this.btnSave.TabIndex = 23;
            this.btnSave.Text = "Speichern";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1079, 556);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.htmlChatHistory);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.extendedUserInput);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.tbxProxyIP);
            this.Controls.Add(this.tbxProxyPort);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxLog);
            this.Controls.Add(this.tbxNick);
            this.Controls.Add(this.tbxChannel);
            this.Controls.Add(this.tbxPw);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnConnect);
            this.Name = "MainForm";
            this.Text = "Kadel";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxPw;
        private System.Windows.Forms.TextBox tbxChannel;
        private System.Windows.Forms.TextBox tbxNick;
        private System.Windows.Forms.TextBox tbxLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxProxyPort;
        private System.Windows.Forms.TextBox tbxProxyIP;
        private System.Windows.Forms.Button btnDisconnect;
        private KDF.Graphics.ExtendedInput extendedUserInput;
        private System.Windows.Forms.Panel panel1;
        private KDF.Graphics.UserList.UserListControl userListControl;
        private KDF.Graphics.HTMLChatHistory htmlChatHistory;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
    }
}

