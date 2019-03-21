using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KDF.ClientEventArgs;

namespace KDF.Security
{
    public class SecurityWatcher
    {
        #region Events
        /// <summary>
        /// Dieses Event wird ausgelöst wenn eine Adminaktivität stattfindet
        /// </summary>
        public event EventHandler<SecurityWatcherEventArgs> OnAction;

        /// <summary>
        /// Dieses Event wird ausgelöst wenn ein Notruf erhalten wurde
        /// </summary>
        public event EventHandler<EmergencyCallEventArgs> OnEmergencyCall;
        #endregion

        #region Returns
        private List<string> _adminlist;
        /// <summary>
        /// Ruft die Adminlist ab
        /// </summary>
        public List<string> Adminlist
        {
            get { return _adminlist; }
        }

        /// <summary>
        /// Gibt an ob die Adminlist geladen wurde
        /// </summary>
        public bool AdminlistIsLoaded
        {
            get { return _adminlist != null && _adminlist.Count != 0; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parsed den Tokenstring und löst ein Event aus
        /// </summary>
        /// <param name="data">Tokenstring der geparsed werden soll</param>
        public void Parse(string data)
        {
            string[] token = data.Split('\0');
            switch (token[0])
            {
                case "e":
                    if (AdminlistIsLoaded && Adminlist.Contains(token[1]))
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(SecurityWatcherEventArgs.SecurityAlertReason.Public, token[1], token[3], 0, token[2]));
                    break;

                case "r":
                    if (token[4].Contains("Überprüfung auf Bot-Benutzung"))
                    {
                        int code = int.Parse(Regex.Match(data, "/ok (\\d+)").Groups[1].Value);
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(SecurityWatcherEventArgs.SecurityAlertReason.BotCheck, "James", null, code, token[3]));
                    }

                    if (AdminlistIsLoaded && Adminlist.Contains(token[1]))
                    {
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(SecurityWatcherEventArgs.SecurityAlertReason.Private, token[1], token[4], 0, token[3]));
                    }

                    if (token[4].Contains("hat dir gerade eine Nachricht") && token[4].Contains("geschickt."))
                    {
                        string[] text = token[4].Split('°');
                        string nick = null;
                        foreach  (string s in text)
                            if (s.Contains(">_h") && s.Contains("|/serverpp"))
                                nick = s.Replace(">_h", string.Empty).Split('|')[0];    
                        
                        if (AdminlistIsLoaded && Adminlist.Contains(nick))
                            if (OnAction != null)
                                OnAction(this, new SecurityWatcherEventArgs(string.Format("/m ?{0}", nick)));
                    }

                    if (AdminlistIsLoaded && data.Contains("hat dich in ein _Separee eingeladen") && Adminlist.Contains(token[4].Replace("_°>_h", "").Split('|')[0]))
                    {
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(SecurityWatcherEventArgs.SecurityAlertReason.Invite, token[4].Replace("_°>_h", "").Split('|')[0], null, int.Parse(data.Substring(data.IndexOf("/ok")).Split('|')[0].Replace("/ok", string.Empty).Trim()), token[3]));
                    }
                    if (data.Contains("Es ist eine _Beschwerde"))
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(data.Substring(0, data.IndexOf("<r°_")).Split('|')[1]));
                    break;

                case "l":
                    if (AdminlistIsLoaded && Adminlist.Contains(token[2]))
                        if (OnAction != null)
                            OnAction(this, new SecurityWatcherEventArgs(SecurityWatcherEventArgs.SecurityAlertReason.Joined, token[2]));
                    break;

                case "k":
                    string title = token[1].Split('õ')[0];
                    if (title.Contains("Hilfe") && !AdminlistIsLoaded)
                        LoadAdminlist(data);
                    else if (title.Contains("Dein Briefkasten"))
                        MailParser(data);
                    else if (title.Contains("Notruf-Fall"))
                    {
                        string casenumber = data.Substring(data.IndexOf('*') + 1).Split(' ')[0];
                        string cdiaID = data.Substring(data.IndexOf("õscdiaõ") + 7).Split('õ')[0];
                        string reason = data.Substring(data.IndexOf("Grund") + 8).Split('#')[0];
                        string user = data.Substring(data.IndexOf("Beschwerdeführer:_ _°BB>_h") + 26).Split('|')[0];
                        string usercomment = data.Substring(data.IndexOf("\"<°:_##") + 7).Split('õ')[0];
                        if (OnEmergencyCall != null)
                            OnEmergencyCall(this, new EmergencyCallEventArgs(casenumber, cdiaID, reason, user, usercomment));
                    }
                    break;
            }
        }
        #endregion

        #region Private Methods
        private void MailParser(string data)
        {
            string sender = data.Substring(data.IndexOf("von ") + 4).Split(')')[0];
            if (AdminlistIsLoaded && Adminlist.Contains(sender))
            {
                string msg = data.Substring(data.IndexOf("509°") + 4).Split(new string[] { "#°+5000°" }, StringSplitOptions.None)[0];
                string sig = "";
                if (msg.Contains("°° §#°05° °>layout/hr_over-sg.png<°#°05° #°+701012°°>{globalopacity}50<°"))
                {
                    string[] basic = msg.Split(new string[] { "°° §#°05° °>layout/hr_over-sg.png<°#°05° #°+701012°°>{globalopacity}50<°" }, StringSplitOptions.None);
                    msg = basic[0];
                    sig = basic[1].Replace("°>{globalopacity}100<°", "");
                }
                string subject = data.Substring(data.IndexOf("#_") + 2).Split('_')[0];
                if (OnAction != null)
                    OnAction(this, new SecurityWatcherEventArgs(sender, subject, msg, sig));
            }            
        }

        private void LoadAdminlist(string data)
        {
            _adminlist = new List<string>();
            foreach (Match m in Regex.Matches(data, "°>_h([^\\|]+)"))
            {
                _adminlist.Add(m.Value.Replace("°>_h", String.Empty).Replace("\\", String.Empty));
            }
        }
        #endregion
    }
}
