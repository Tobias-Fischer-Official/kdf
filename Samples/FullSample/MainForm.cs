using System;
using System.Windows.Forms;
using KDF.Networks.Core;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using KDF.Networks.GameServer;
using System.Threading;
using KDF.ClientEventArgs;

namespace KDFFullSample
{
    public partial class MainForm : Form
    {
        private KnuddelsClient _client;
        bool loggedout = false;
        private Dictionary<string, string> Config = new Dictionary<string, string>();


        public MainForm()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(MainForm_FormClosed);
            PrepareClient();
        }

        void PrepareClient()
        {
            //Config = KDF.Features.Data.LoadConfig("133Sieben");
            if (Config != null)
            {
                if (Config.ContainsKey("Password") && Config.ContainsKey("Channel") && Config.ContainsKey("Username"))
                {
                    tbxPw.Text = Config["Password"];
                    tbxChannel.Text = Config["Channel"];
                    tbxNick.Text = Config["Username"];
                }
            }
            else
                Config = new Dictionary<string, string>();

            _client = new KnuddelsClient(this);

            _client.UseKDFGui(extendedUserInput, userListControl, htmlChatHistory);

            _client.OnChangeUserListImage += _client_OnChangeUserListImage;
            _client.OnChannelBackGroundChanged += _client_OnChannelBackGroundChanged;
            _client.OnChannelChangedLayout += _client_OnChannelChangedLayout;
            _client.OnChannelJoined += _client_OnChannelJoined;
            _client.OnDataReceived += _client_OnDataReceived;
            _client.OnGlobalChannelListReceived += _client_OnGlobalChannelListReceived;
            _client.OnConnectionStateChanged += _client_OnConnectionStateChanged;
            _client.OnLoginFailed += _client_OnLoginFailed;
            _client.OnOpenBrowserWindow += _client_OnOpenBrowserWindow;
            _client.OnPrivateMessageReceived += _client_OnPrivateMessageReceived;
            _client.OnPublicMessageReceived += _client_OnPublicMessageReceived;
            _client.OnUserJoinedChannel += _client_OnUserJoinedChannel;
            _client.OnUserLeftChannel += _client_OnUserLeftChannel;
            _client.OnUserListReceived += _client_OnUserListReceived;
            _client.OnWindowOpened += _client_OnWindowOpened;
            _client.OnGlobalException += _client_OnGlobalException;
            _client.OnChatComponentCommandReceived += _client_OnChatComponentCommandReceived;
            _client.OnCardServerConnectionEstablished += _client_OnCardServerConnection;

        }

        void tbxInput_OnReturnPressed(object sender, KDF.Graphics.OnReturnPressedEventArgs e)
        {
            _client.SendUserInput(extendedUserInput.Text);
        }

        void _client_OnChatComponentCommandReceived(object sender, ChatComponentReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("Received \"{0}\" ChatComponentString witch was {1} \r\n", e.Module.Name, string.Join("->", e.Module.Values.Values)));
        }

        void _client_OnGlobalException(object sender, GlobalExceptionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tbxLog.AppendText(string.Format("An Error in Module {0} occured: {1} \r\n", e.KdfModule.ToString(), e.Exception.ToString()));
            });
        }

        void _client_OnWindowOpened(object sender, WindowOpenedEventArgs e)
        {
            new Thread(delegate()
            {
                Form frm = new Form();
                frm.Text = e.PopupParser.WindowTitle;
                frm.BackColor = e.PopupParser.BackColor;

                WebBrowser wb = new WebBrowser();
                wb.DocumentText = e.PopupParser.HTML;
                wb.Dock = DockStyle.Fill;
                frm.Controls.Add(wb);
                frm.ShowDialog();
            })
            {
                ApartmentState = ApartmentState.STA
            }.Start();
            tbxLog.AppendText(string.Format("A window was opened by the server, title:{0}, the windowtoken has {1} bytes \r\n", e.WindowTitle, e.WindowToken.Length));
        }

        void _client_OnUserListReceived(object sender, UserListReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("Userlist successfully received, it contains {0} Users, User[0] = {1}\r\n", e.UserList.ChannelUserList.Count, e.UserList.ChannelUserList[0].Name));
        }

        void _client_OnUserLeftChannel(object sender, UserLeftChannelEventArgs e)
        {
            tbxLog.AppendText(string.Format("An user has left channel '{0}', the Username was {1}\r\n", e.UserLeftChannel.ChannelLeft, e.UserLeftChannel.Name));
        }

        void _client_OnUserJoinedChannel(object sender, UserJoinedChannelEventArgs e)
        {
            tbxLog.AppendText(string.Format("An user has joined channel '{0}', the Username was {1}\r\n", e.ChannelUser.ChannelJoined, e.ChannelUser.Name));
        }

        void _client_OnPublicMessageReceived(object sender, PublicMessageReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("A public message in channel '{0}' was send from '{1}', the message was '{2}' bytes long\r\n", e.PublicMessage.Channel, e.PublicMessage.Sender, e.PublicMessage.Message.Length));
        }

        void _client_OnPrivateMessageReceived(object sender, PrivateMessageReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("A private message out of channel '{0}' was send from '{1}' to channel {2}, the message was '{3}' bytes long\r\n", e.PrivateMessage.FromChannel, e.PrivateMessage.Sender, e.PrivateMessage.ToChannel, e.PrivateMessage.Message.Length));
        }

        void _client_OnOpenBrowserWindow(object sender, OpenBrowserWindowEventArgs e)
        {
            tbxLog.AppendText(string.Format("The server wants you to open '{0}' in a browserwindow\r\n", e.Url));
        }

        void _client_OnLoginFailed(object sender, LoginFailedEventArgs e)
        {
            tbxLog.AppendText(string.Format("The login failed, for the reason '{0}'\r\n", e.Reason.ToString()));
        }

        void _client_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (!loggedout)
                {
                    tbxLog.AppendText(string.Format("The connectionstate changed, the client is {0} connected and {1} logged in\r\n", e.Connected ? "" : "not", e.LoggedIn ? "" : "not"));
                    loggedout = true;
                }
            });
        }

        void _client_OnGlobalChannelListReceived(object sender, GlobalChannelListReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("The global channellist was recently received, it contains {0} channels\r\n", e.GlobalChannelList.Count));
        }

        void _client_OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("Data was received\r\n"));
            if (e.Data.StartsWith("6"))
                tbxLog.AppendText(e.Data.Replace("\0", "\\0"));
        }

        void _client_OnChannelJoined(object sender, ChannelJoinedEventArgs e)
        {
            tbxLog.AppendText(string.Format("Your client joined successfully in '{0}'\r\n", e.JoinedChannel.Name));
        }

        void _client_OnChannelChangedLayout(object sender, ChannelChangedLayoutEventArgs e)
        {
            tbxLog.AppendText(string.Format("The channel '{0}' got its layout\r\n", e.channelWithNewLayout.Name));
        }

        void _client_OnChannelBackGroundChanged(object sender, ChannelBackGroundChangedEventArgs e)
        {
            tbxLog.AppendText(string.Format("The channel '{0}' got a new background\r\n", e.ChannelName));
        }

        void _client_OnChangeUserListImage(object sender, ChangeUserListImageEventArgs e)
        {
            tbxLog.AppendText(string.Format("The userlistimage '{0}' from user '{1}' in channel '{2}' was {3}", e.UserListImage.Image, e.UserListImage.User, e.UserListImage.Channel, e.AddOrRemove ? "added\r\n" : "removed\r\n"));
        }

        #region CardServer
        void _client_OnCardServerConnection(object sender, GameServerConnectionEventArgs e)
        {
            tbxLog.AppendText(string.Format("CardServer Client created, [ Host: '{0}', Port: '{1}', Channel: '{2}', GameId: '{3}' ]\r\n", e.Client.ServerHost, e.Client.ServerPort, e.Client.Channel, e.Client.GameId));

            e.Client.OnConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(_cardClient_OnConnectionStateChanged);
            e.Client.OnModuleReceived += new EventHandler<ModuleReceivedEventArgs>(_cardClient_OnModuleReceived);

            e.Client.Connect();
        }

        void _cardClient_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            GSClient client = (GSClient)sender;

            tbxLog.AppendText(string.Format("The connectionstate of CardServer-Client changed, the client is {0} connected and {1} logged in\r\n", client.Connected ? "" : "not", client.LoggedIn ? "" : "not"));

            if (e.Connected && !e.LoggedIn)
                client.Login();
            else if (e.Connected && e.LoggedIn)
                client.JoinRoom();
        }

        void _cardClient_OnModuleReceived(object sender, ModuleReceivedEventArgs e)
        {
            tbxLog.AppendText(string.Format("Received \"{0}\" CardServerModule witch was {1} \r\n", e.Module.Name, string.Join("->", e.Module.Values.Values)));
        }
        #endregion

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _client.Connect(ChatSystem.de);
            //_client.Connect(ChatSystem.de, int.Parse(tbxProxyPort.Text), tbxProxyIP.Text);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            _client.Login(tbxNick.Text, tbxPw.Text, tbxChannel.Text);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
            PrepareClient();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Config = KDF.Features.Data.LoadConfig(tbxNick.Text);
            tbxPw.Text = Config["Password"];
            tbxChannel.Text = Config["Channel"];
            tbxNick.Text = Config["Username"];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Config.ContainsKey("Password"))
                Config["Password"] = tbxPw.Text;
            else
                Config.Add("Password", tbxPw.Text);

            if (Config.ContainsKey("Username"))
                Config["Username"] = tbxNick.Text;
            else
                Config.Add("Username", tbxNick.Text);

            if (Config.ContainsKey("Channel"))
                Config["Channel"] = tbxChannel.Text;
            else
                Config.Add("Channel", tbxChannel.Text);

            KDF.Features.Data.SaveConfig(Config);
        }
    }
}