using KDF.Graphics;
using KDF.Graphics.UserList;
using KDF.Networks.Core;
using KDF.ChatObjects;
using System.Collections.Generic;

namespace KDF.Helper
{
    public class GuiConnector
    {
        #region public vars

        private ExtendedInput _exInput;
        public ExtendedInput ExInput
        {
            get { return _exInput; }
            set { _exInput = value; }
        }

        private UserListControl _userList;
        public UserListControl UserList
        {
            get { return _userList; }
            set { _userList = value; }
        }

        private KnuddelsClient _client;
        public KnuddelsClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        private HTMLChatHistory _htmlChatHistory;
        public HTMLChatHistory HtmlChatHistory
        {
            get { return _htmlChatHistory; }
            set { _htmlChatHistory = value; }
        }

        private bool _active = false;
        public bool Active
        {
            get { return _active; }
        }

        #endregion

        #region private vars

        private Dictionary<string, Channel> _channels = new Dictionary<string, Channel>();
        private string _activeChannel;

        #endregion

        public GuiConnector(ExtendedInput exInput, UserListControl userList, HTMLChatHistory htmlChatHistory, KnuddelsClient client)
        {
            _exInput = exInput;
            _exInput.Enabled = false;
            _userList = userList;
            _htmlChatHistory = htmlChatHistory;
            _client = client;
        }

        #region UserList bezogen

        public void AddUserList(Channel channel)
        {
            if (!_channels.ContainsKey(channel.Name))
            {
                _channels.Add(channel.Name, channel);
                _activeChannel = channel.Name;
                SelectChannel(channel.Name);                
            }
        }

        public void AddUserToUserList(ChannelUser user)
        {
            if (_activeChannel == user.ChannelJoined)
                _userList.AddUser(user, _channels[_activeChannel].BackColor);
        }

        public void RemoveUserFromUserList(UserLeftChannel user)
        {
            if (_activeChannel == user.ChannelLeft)
                _userList.RemoveUser(user.Name);
        }

        void _userList_OnItemRightClickShifted(object sender, ItemRightClickShiftedEventArgs e)
        {
            _exInput.Text += e.Message + ' ';
        }

        #endregion

        #region global (alle controls)

        public void DeleteChannel(string channel)
        {
            _channels.Remove(channel);
            if (_activeChannel == channel)
            {
                //Userliste Leeren
                _userList.Clear();
                //Chatverlauf leeren


                //Anderen Channel anzeigen
                if (_channels.Count >= 1)
                {
                    string[] tempChannelNames = { };
                    _channels.Keys.CopyTo(tempChannelNames, 0);
                    SelectChannel(tempChannelNames[0]);
                }
                //wenn keiner übrig ist, das input deaktivieren
                else
                {
                    _exInput.Enabled = false;
                }
            }
        }

        public void SelectChannel(string channel)
        {
            if (_channels.ContainsKey(channel))
            {
                //Userliste füllen/auswählen
                _userList.Clear();
                _userList.FillNickList(_channels[channel]);
                //Chatverlauf füllen/auswählen

                //EscapeListe der Textbox aktualisieren
                _exInput.AutoCompleteEntrys.Clear();
                _exInput.AutoCompleteEntrys.AddRange(_userList.AllUsers.Keys);
            }
        }

        #endregion

        #region chatverlauf bezogen

        public void AddLineToHistory(PublicMessage msg, bool function)
        {
            _htmlChatHistory.Append(msg, _channels[_activeChannel], function);
        }

        #endregion

        #region input bezogen

        void _exInput_OnReturnPressed(object sender, OnReturnPressedEventArgs e)
        {
            if (e.OutData == "/clear" || e.OutData == "/cls" || e.OutData == "/clr")
                _htmlChatHistory.Clear();
            if (e.OutData.StartsWith("/w"))
            {
                string first = "q\0´\ninfoSystemslash:/w\0\0\0";
                string second = string.Empty;
                string nick = e.OutData.Replace("/w", string.Empty);
                if (nick.StartsWith(" "))
                {
                    second = "q\0H133Sieben\0Start\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0ÿÿÿÿ\0\0ÿ\0\0	textField";
                }
                else if(nick == string.Empty)
                {
                    second = "q\0133Sieben\0Start\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0ÿÿÿÿ\0\0ÿ\0\0	textField";                    
                }
                _client.Send(first);
                _client.Send(second);

            }
            else
                _client.SendUserInput(_client.ClientUser.SelectedChannel.Name, e.OutData);
        }

        #endregion

        #region interne funktionalität

        public void Activate(Channel channel)
        {
            if (!_active)
            {
                _active = true;
                _exInput.Enabled = true;
                _exInput.OnReturnPressed += new System.EventHandler<OnReturnPressedEventArgs>(_exInput_OnReturnPressed);
                _userList.OnItemRightClickShifted += new System.EventHandler<ItemRightClickShiftedEventArgs>(_userList_OnItemRightClickShifted);
                _htmlChatHistory.OnLinkClicked += new System.EventHandler<System.Windows.Forms.LinkClickedEventArgs>(_htmlChatHistory_OnLinkClicked);
                _htmlChatHistory.Activate(channel);
            }
        }

        void _htmlChatHistory_OnLinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            _client.SendUserInput(e.LinkText);
        }

        #endregion



    }
}
