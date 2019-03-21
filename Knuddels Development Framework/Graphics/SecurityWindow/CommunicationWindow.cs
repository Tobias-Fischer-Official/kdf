using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KDF.Networks.Core;
using KDF.Security;
using KDF.ClientEventArgs;
using System.Threading;

namespace KDF.Graphics.SecurityWindow
{
    public partial class CommunicationWindow : Form
    {
        #region Enum
        enum SendingOption
        {
            EmergencyCall,
            Mail,
            Private,
            Public,
            Default
        }
        #endregion

        #region private Vars
        private SecurityWatcher _securityWatcher = null;
        private KnuddelsClient _client = null;
        private bool _securityActive = false;
        private bool _showSecurityGUI = false;
        private SendingOption sop = SendingOption.Default;
        private string nick = null;
        private string info = null;
        #endregion

        #region Konstruktor
        /// <summary>
        /// Erstellt das SecurityWatcher-Fenster in dem bereits ein SecurityWatcher einbaut wurde
        /// </summary>
        /// <param name="Client">Der zu übergebene Client, damit der SecurityWatcher arbeiten kann</param>
        /// <param name="securityActive">Ob der SecurityWatcher überwachen soll, wenn ja (true) übergeben</param>
        /// <param name="showSecurityGUI">Ob das Fenster bei Admin-Aktivität angezeigt werden soll, wenn ja (true) übergeben</param>
        public CommunicationWindow(KnuddelsClient Client, bool securityActive, bool showSecurityGUI)
        {
            InitializeComponent();
            _client = Client;
            _client.OnDataReceived += _client_OnDataReceived;
            _securityActive = securityActive;
            _showSecurityGUI = showSecurityGUI;
            _securityWatcher = new SecurityWatcher();
            _securityWatcher.OnAction += _securityWatcher_OnAction;
            _securityWatcher.OnEmergencyCall += _securityWatcher_OnEmergencyCall;
            this.Visible = false;
        }
        #endregion

        #region Events
        void _client_OnDataReceived(object sender, ClientEventArgs.DataReceivedEventArgs e)
        {
            if (!_securityWatcher.AdminlistIsLoaded)
                if (e.Data[0] == 'u')
                    _client.SendUserInput("/h");

            if (_securityActive)
                _securityWatcher.Parse(e.Data);
        }

        void _securityWatcher_OnEmergencyCall(object sender, ClientEventArgs.EmergencyCallEventArgs e)
        {
            if (_showSecurityGUI)
            {
                sop = SendingOption.EmergencyCall;
                info = e.cdiaID;
                AddLog(string.Format("<Notruf> cdiaID: {0}", e.cdiaID));
                AddLog(string.Format("<Notruf> Fallnummer: {0}", e.Casenumber));
                AddLog(string.Format("<Notruf> Grund: {0}", e.Reason));
                AddLog(string.Format("<Notruf> Beschwerdeführer: {0}", e.User));
                AddLog(string.Format("<Notruf> Userkommentar: {0}", e.Usercomment));
                Show();
            }
        }

        void _securityWatcher_OnAction(object sender, ClientEventArgs.SecurityWatcherEventArgs e)
        {
            switch (e.Reason)
            {
                case SecurityWatcherEventArgs.SecurityAlertReason.BotCheck:
                    int code = e.Code;
                    sop = SendingOption.Default;
                    new Thread(delegate()
                    {
                        Thread.Sleep(new Random().Next(20000, 60000));
                        _client.SendUserInput(string.Format("/ok {0}", code));
                    }).Start();
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.SendUserInputForMe:
                    _client.SendUserInput(e.ToSend);
                    sop = SendingOption.Default;
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.Private:
                    if (_showSecurityGUI)
                    {
                        sop = SendingOption.Private;
                        nick = e.AdminNick;
                        AddLog(string.Format("<Private> {0}: {1}", e.AdminNick, e.Message));
                        Show();
                    }
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.Invite:
                    int Code = e.Code;
                    sop = SendingOption.Default;
                    new Thread(delegate()
                    {
                        Thread.Sleep(new Random().Next(5000, 20000));
                        _client.SendUserInput(string.Format("/ok {0}", Code));
                    }).Start();
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.Joined:
                    if (_showSecurityGUI)
                    {
                        sop = SendingOption.Default;
                        AddLog(string.Format("<Joined> Der Admin {0} hat den Channel betreten", e.AdminNick));
                        Show();
                    }
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.Mail:
                    if (_showSecurityGUI)
                    {
                        sop = SendingOption.Mail;
                        nick = e.AdminNick;
                        info = e.Subject;
                        AddLog(string.Format("<Mail> Sender: {0}", e.AdminNick));
                        AddLog(string.Format("<Mail> Betreff: {0}", e.Subject));
                        AddLog(string.Format("<Mail> Nachricht: {0}", e.Message));
                        AddLog(string.Format("<Mail> Signatur: {0}", e.Signature));
                        Show();
                    }
                    break;

                case SecurityWatcherEventArgs.SecurityAlertReason.Public:
                    if (_showSecurityGUI)
                    {
                        sop = SendingOption.Public;
                        info = e.Channel;
                        AddLog(string.Format("<Public> {0}: {1}",e.AdminNick, e.Message));
                        Show();
                    }          
                    break;
            }
        }

        private void tbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (tbxInput.Text != null && tbxInput.Text != string.Empty)
                {
                    switch (sop)
                    {
                        case SendingOption.Public:
                            {
                                _client.Send(string.Format("e\0{0}\0{1}\0{2}\0 ", _client.ClientUser.Username, info, tbxInput.Text));
                                AddLog(string.Format("<Public> {0}: {1}", _client.ClientUser.Username, tbxInput.Text));
                            }
                            break;

                        case SendingOption.Private:
                            {
                                _client.SendUserInput(string.Format("/p {0}: {1}", nick, tbxInput.Text));
                                AddLog(string.Format("<Private-Antwort an {0}> {1}: {2}", nick, _client.ClientUser.Username, tbxInput.Text));
                            }
                            break;

                        case SendingOption.EmergencyCall:
                            if (tbxInput.Text.Length > 40)
                            {
                                _client.Send(string.Format("cdia\0{0}\0    OK    \00\01\0{1}\0", info, tbxInput.Text));
                                AddLog(string.Format("<Notruf-Antwort> {0}", tbxInput.Text));
                            }
                            else
                                AddLog("<Notruf> Deine Antwort muss mehr als 40 Zeichen haben!");
                            break;

                        case SendingOption.Mail:
                            {
                                _client.Send(string.Format("post\0post\0Senden\0{0}\0Re: {1}\0-1:NoIcon\00\01\0{2}\0", nick, info, tbxInput.Text));
                                AddLog(string.Format("<Mail-Antwort an {0}> {1}: {2}", nick, _client.ClientUser.Username, tbxInput.Text));
                            }
                            break;

                        case SendingOption.Default:
                            //Default der Standard damit selbst wenn man was eingibt nix falsches Sendet
                            break;
                    }
                    tbxInput.Text = "";
                }
            }
        }
        #endregion

        #region Private Methods
        private void AddLog(string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                rtbxLog.AppendText(text + Environment.NewLine);
                rtbxLog.ScrollToCaret();
            });
        }

        private void Show()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Visible = true;
                rtbxLog.ScrollToCaret();
            });
        }
        #endregion

        #region Override OnClosing
        protected override void OnClosing(CancelEventArgs e)
        {
            sop = SendingOption.Default;
            nick = null;
            info = null;
            this.Visible = false;
            e.Cancel = true;
        }
        #endregion
    }
}
