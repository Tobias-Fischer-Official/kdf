using KDF.ChatObjects;
using KDF.ChatObjects.Collections;
using KDF.ClientEventArgs;
using KDF.Exceptions;
using KDF.Graphics;
using KDF.Graphics.UserList;
using KDF.HelperClasses;
using KDF.HelperClasses.Parser;
using KDF.Networks.GameServer;
using KDF.Networks.Protocol.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;



namespace KDF.Networks.Core
{
    #region ChatSystem
    /// <summary>
    /// Das gewünschte Chatsystem, zu welchem verbunden werden soll
    /// </summary>
    public enum ChatSystem
    {
        /// <summary>
        /// Der deutsche Chatserver + Port
        /// </summary>
        de = 2710,
        /// <summary>
        /// Der amerikanische Chatserver + Port
        /// </summary>
        com = 2713,
        /// <summary>
        /// Der östereichische Chatserver + Port
        /// </summary>
        at = 2711,
        /// <summary>
        /// Der schweizer Chatserver + Port
        /// </summary>
        ch = 2712,
        /// <summary>
        /// Der Mainfranken Chatserver + Port
        /// </summary>
        mfc = 4242,
        /// <summary>
        /// Der zum Testen gedachte Chatserver + Port
        /// </summary>
        test = 2720
    }
    #endregion

    #region ClientType
    /// <summary>
    /// Der gewünschte Client Type
    /// </summary>
    public enum ClientType
    {
        Normal,
        iPhone
    }
    #endregion

    /// <summary>
    /// Der Client der zum jeweiligen Chatserver von Knuddels verbindet
    /// </summary>
    /// <example>
    /// <code>
    /// using System;
    /// using KDF.Networks.Core;
    ///
    ///namespace KDF_Console_Sample
    ///{
    ///    class Program
    ///    {
    ///        static void Main(string[] args)
    ///        {
    ///            Client c = new Client();
    ///            c.OnDataReceived += new EventHandler&lt;KDF.ClientEventArgs.DataReceivedEventArgs&gt;(c_OnDataReceived);
    ///            c.Connect(ChatSystem.de);
    ///            while (!c.Connected) ;            
    ///            c.ServerCommands.Login("", "", "");
    ///        }
    ///        static void c_OnDataReceived(object sender, KDF.ClientEventArgs.DataReceivedEventArgs e)
    ///        {
    ///            Console.WriteLine(e.Data.Length);
    ///        }
    ///    }
    ///}
    /// </code>
    /// </example>
    public class KnuddelsClient
    {
        #region Constants
        /// <summary>
        /// Die Autoren der Library
        /// </summary>
        public const string Authors = "Sky.NET, 3Lit, SeBi, Grammatikfehler";
        /// <summary>
        /// Die aktuelle Version der Library
        /// </summary>
        public const string Version = "3.0";
        /// <summary>
        /// Die übergeordnete Organisation, welche die Library publiziert
        /// </summary>
        public const string Company = "Spin-Solutions";
        private const char _nullChar = '\0';
        #endregion

        #region Public Vars

        private ClientUser _clientUser;
        /// <summary>
        /// Ruft den aktuellen User des Clients ab
        /// </summary>
        public ClientUser ClientUser
        {
            get { return _clientUser; }
            set { _clientUser = value; }
        }

        private Form _formToInvoke;
        /// <summary>
        /// Gibt die Form an welche Invoked werden soll, wenn eine Nachricht empfangen wird
        /// </summary>
        public Form FormToInvoke
        {
            get { return _formToInvoke; }
            set { _formToInvoke = value; }
        }

        private Window _windowToInvoke;
        /// <summary>
        /// Gibt die Form an welche Invoked werden soll, wenn eine Nachricht empfangen wird
        /// </summary>
        public Window WindowToInvoke
        {
            get { return _windowToInvoke; }
            set { _windowToInvoke = value; }
        }

        private string _systemButler = null;
        /// <summary>
        /// Der Name des Chatbutlers des Chatservers zu welchem sich der Client verbunden hat
        /// </summary>
        public string SystemButler
        {
            get { return _systemButler; }
        }
        /// <summary>
        /// Ruft ab ob der Client mit dem Chatserver verbunden ist
        /// </summary>
        public bool Connected
        {
            get { return _connection.Connected; }
        }

        private bool _loggedIn = false;
        /// <summary>
        /// Gibt an ob der Client mit einem Nicknamen und Passwort an dem Chatserver, mit welchem er verbunden ist, angemeldet ist
        /// </summary>
        public bool LoggedIn
        {
            get { return _loggedIn; }
        }

        private int _serverPort = 0;
        /// <summary>
        /// Gibt den aktuellen Port des Servers an mit dem der Client verbunden ist
        /// </summary>
        public int ServerPort
        {
            get { return _serverPort; }
        }

        private string _serverHost = string.Empty;
        /// <summary>
        /// Gibt den aktuellen Host des Servers an mit dem der Client verbunden ist
        /// </summary>
        public string ServerHost
        {
            get { return _serverHost; }
        }

        public string ProxyHost
        {
            get
            {
                if (_connection == null)
                    return null;
                return _connection.ProxyIp;
            }
        }
        public int ProxyPort
        {
            get
            {
                if (_connection == null)
                    return 0;
                return _connection.ProxyPort;
            }
        }


        private ClientType _clientType = ClientType.Normal;
        /// <summary>
        /// Gibt den aktuellen Type des Clients aus oder legt diesen fest (Wenn der Client noch nicht verbunden ist)
        /// </summary>
        public ClientType ClientType
        {
            get { return _clientType; }
            set { if (!Connected) { _clientType = value; } }
        }

        private Dictionary<string, GSClient> _cardServerClients = new Dictionary<string, GSClient>();
        /// <summary>
        /// Gibt einen Instanz der CSClient-Klasse zurück welche dem Channel zugeordnet ist
        /// </summary>
        public Dictionary<string, GSClient> CardServerClients
        {
            get { return _cardServerClients; }
        }

        #region eventhandler
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein User in der Userliste ein Bild erhält oder verliert
        /// </summary>
        public event EventHandler<ChangeUserListImageEventArgs> OnChangeUserListImage;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein Channel ein neues LAyout zugewiesen bekommt
        /// </summary>
        public event EventHandler<ChannelChangedLayoutEventArgs> OnChannelChangedLayout;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn der Client daten empfangen hat
        /// </summary>
        public event EventHandler<KDF.ClientEventArgs.DataReceivedEventArgs> OnDataReceived;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn sich der Zustand des Logins ändert
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn der Login fehlschlägt
        /// </summary>
        public event EventHandler<LoginFailedEventArgs> OnLoginFailed;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn der Server dem Client eine Website mitteilt, welche dieser öffnen soll
        /// </summary>
        public event EventHandler<OpenBrowserWindowEventArgs> OnOpenBrowserWindow;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn der mit dem Client eingeloggte User eine private Nachricht enthällt
        /// </summary>
        public event EventHandler<PrivateMessageReceivedEventArgs> OnPrivateMessageReceived;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn Jemand in einem Channel eine öffentliche Nachricht schreibt, in welchem der mit dem Client eingeloggte User sich befindet
        /// </summary>
        public event EventHandler<PublicMessageReceivedEventArgs> OnPublicMessageReceived;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein User einen Channel betritt, in welchem sich der mit dem Client eingeloggte User befindet
        /// </summary>
        public event EventHandler<UserJoinedChannelEventArgs> OnUserJoinedChannel;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn die globale Channelliste empfangen wurde
        /// </summary>
        public event EventHandler<GlobalChannelListReceivedEventArgs> OnGlobalChannelListReceived;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn der mit dem Client eingeloggte USer einen Channel auf dem Chatserver betritt
        /// </summary>
        public event EventHandler<ChannelJoinedEventArgs> OnChannelJoined;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn man die Userliste eines Channels empfängt
        /// </summary>
        public event EventHandler<UserListReceivedEventArgs> OnUserListReceived;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn das Hintergrundbild eines Channels geändert werden soll
        /// </summary>
        public event EventHandler<ChannelBackGroundChangedEventArgs> OnChannelBackGroundChanged;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein User einen Channel verlässt
        /// </summary>
        public event EventHandler<UserLeftChannelEventArgs> OnUserLeftChannel;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein Fenster geöffnet wurde
        /// </summary>
        public event EventHandler<WindowOpenedEventArgs> OnWindowOpened;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein Fehler in einem der Framework-Module auftritt
        /// </summary>
        public event EventHandler<GlobalExceptionEventArgs> OnGlobalException;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn ein :-Token empfangen wurde
        /// </summary>
        public event EventHandler<ChatComponentReceivedEventArgs> OnChatComponentCommandReceived;
        /// <summary>
        /// Dieses Event ist zur reinen Übergabe von Objekten, und wird immer ausgelöst, wenn etwas empfangen wurde (sofern es gesetzt wird).
        /// </summary>
        public event EventHandler<ObjectFormedEventArgs> OnObjectFormed;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn eine Verbindung zum CardServer aufgebaut wird
        /// </summary>
        public event EventHandler<GameServerConnectionEventArgs> OnCardServerConnectionEstablished;
        #endregion
        #endregion

        #region private Vars
        private Token2ObjectParser _parser;
        private Connection _connection;
        internal Connection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        private GuiConnector _guiConnector;
        internal KnModule _parentModule;
        private bool _securityWindowIsOpen;
        #endregion

        #region Konstruktor
        /// <summary>
        /// Erstellt eine neue Instanz des Clients
        /// </summary>
        /// <remarks>Bitte diesen Konstruktor nur bei Konsolenanwendungen nutzen</remarks>
        public KnuddelsClient()
        {
            _parser = new Token2ObjectParser(this);
            _connection = new Connection(this);
            _connection._onConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(_connection__onConnectionStateChanged);
            _connection._onGlobalException += new EventHandler<GlobalExceptionEventArgs>(_connection__onGlobalException);
        }

        /// <summary>
        /// Erstellt eine neue Instanz des Clients
        /// </summary>
        /// <param name="formToInvoke">Wenn mit einem Windows-Formsprojekt gearbeitet wird, kann hier die Form übergeben werden, 
        /// welche in einem eigenen GUI-Thread läuft, und daher invoked werden muss</param>
        /// <remarks>Die Verwendung dieses Konstruktors ist bei Konsolenanwendungen nicht nötig bzw. unsinnig</remarks>
        public KnuddelsClient(Form formToInvoke)
        {
            _formToInvoke = formToInvoke;
            _parser = new Token2ObjectParser(this);
            _connection = new Connection(this);
            _connection._onConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(_connection__onConnectionStateChanged);
            _connection._onGlobalException += new EventHandler<GlobalExceptionEventArgs>(_connection__onGlobalException);
        }

        /// <summary>
        /// Erstellt eine neue Instanz des Clients
        /// </summary>
        /// <param name="windowToInvoke">Wenn mit einem WPF-Projekt gearbeitet wird, kann hier das Fenster übergeben werden, 
        /// welches in einem eigenen GUI-Thread läuft, und daher invoked werden muss</param>
        /// <remarks>Die Verwendung dieses Konstruktors ist bei Konsolenanwendungen nicht nötig bzw. unsinnig</remarks>
        public KnuddelsClient(Window windowToInvoke)
        {
            _windowToInvoke = windowToInvoke;
            _parser = new Token2ObjectParser(this);
            _connection = new Connection(this);
            _connection._onConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(_connection__onConnectionStateChanged);
            _connection._onGlobalException += new EventHandler<GlobalExceptionEventArgs>(_connection__onGlobalException);
        }
        #endregion

        #region Events
        void _connection__onGlobalException(object sender, GlobalExceptionEventArgs e)
        {
            if (this.OnGlobalException != null)
                OnGlobalException(this, e);
        }

        void _connection__onConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (this.OnConnectionStateChanged != null)
                OnConnectionStateChanged(this, e);
        }
        #endregion

        #region HandleData
        internal void HandleIncomingData(string data)
        {
            if (_formToInvoke != null && _formToInvoke.InvokeRequired)
            {
                _formToInvoke.Invoke((MethodInvoker)delegate
                {
                    HandleIncomingDataSavely(data);
                });
            }
            else if (_windowToInvoke != null)
            {
                _windowToInvoke.Dispatcher.Invoke((MethodInvoker)delegate
                {
                    HandleIncomingDataSavely(data);
                });
            }
            else
                HandleIncomingDataSavely(data);
        }

        internal void HandleIncomingDataSavely(string data)
        {
            if (data != null)
                try
                {
                    object _object = null;
                    string _type = null;
                    string[] packets = data.Split(_nullChar);
                    string packeID = packets[0];
                    if (OnDataReceived != null)
                        OnDataReceived(this, new KDF.ClientEventArgs.DataReceivedEventArgs(data));

                    switch (packeID)
                    {
                        //-------------------------------------------------------
                        //----------------------Systembezogen--------------------
                        //-------------------------------------------------------
                        case "(":
                            if (OnConnectionStateChanged != null)
                                OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, true));
                            _object = new ConnectionStateChangedEventArgs(false, true);
                            _type = "ConnectionStateChanged";
                            UpdateModule();
                            break;
                        //Wird direkt von der Connection-Klasse verarbeitet
                        case "5":
                            _systemButler = packets[1];
                            break;
                        case ",":
                            Send("h" + _nullChar + (data.Length == 1 ? "-" : data.Substring(2)));
                            break;
                        //-------------------------------------------------------
                        //-----------------------Nachrichten---------------------
                        //-------------------------------------------------------
                        case "e":

                            PublicMessage publicMessage = new PublicMessage(packets, _clientUser.SelectedChannel.Name);
                            if (OnPublicMessageReceived != null)
                                OnPublicMessageReceived(this, new PublicMessageReceivedEventArgs(publicMessage));
                            _object = new PublicMessage(packets, _clientUser.SelectedChannel.Name);
                            _type = "PublicMessageReceived";
                            //if (_guiConnector.Active)
                            //    _guiConnector.AddLineToHistory(publicMessage, true);
                            break;
                        case "r":

                            PrivateMessage p = new PrivateMessage(packets, _clientUser.SelectedChannel.Name);
                            if (!_loggedIn && packets[1] == "James")
                            {
                                _loggedIn = true;
                                if (OnConnectionStateChanged != null)
                                    OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(true, true));
                            }
                            if (OnPrivateMessageReceived != null)
                                OnPrivateMessageReceived(this, new PrivateMessageReceivedEventArgs(p));
                            _object = p;
                            _type = "PrivateMessageReceived";

                            break;
                        case "t":

                            PublicMessage publicMessageF = new PublicMessage(packets, _clientUser.SelectedChannel.Name);
                            if (OnPublicMessageReceived != null)
                                OnPublicMessageReceived(this, new PublicMessageReceivedEventArgs(publicMessageF));
                            _object = new PublicMessage(packets, _clientUser.SelectedChannel.Name);
                            _type = "PublicFunctionReceived";
                            //if (_guiConnector.Active)
                            //    _guiConnector.AddLineToHistory(publicMessageF, true);
                            break;
                        //-------------------------------------------------------
                        //-----------------------Channelbezogen------------------
                        //-------------------------------------------------------
                        case "b":
                            if (OnGlobalChannelListReceived != null)
                                OnGlobalChannelListReceived(this, new GlobalChannelListReceivedEventArgs(_parser.ParseGlobalChannelList(data)));
                            _object = _parser.ParseGlobalChannelList(data);
                            _type = "GlobalChannelListReceived";
                            break;
                        case "a":
                            System.Diagnostics.Debug.WriteLine(data.Replace("\0", "\\0"));
                            Channel joinedChannel = _parser.ParseChannel(data, false);
                            if (!this._clientUser.OnlineChannels.ContainsKey(joinedChannel.Name))
                                this._clientUser.OnlineChannels.Add(joinedChannel.Name, joinedChannel);
                            else
                                this._clientUser.OnlineChannels[joinedChannel.Name] = joinedChannel;
                            if (OnChannelJoined != null)
                                OnChannelJoined(this, new ChannelJoinedEventArgs(joinedChannel));
                            ChangeSelectedChannel(joinedChannel.Name);
                            _object = joinedChannel;
                            _type = "ChannelJoined";
                            //if (!_guiConnector.Active)
                            //    _guiConnector.Activate(joinedChannel);
                            break;
                        case "d":
                            _clientUser.OnlineChannels.Remove(packets[1]);
                            Channel joined = new Channel(packets[2]);
                            _clientUser.OnlineChannels.Add(packets[2], joined);
                            _clientUser.SelectedChannel = joined;
                            _object = packets[1] + "|" + packets[2];
                            _type = "ChannelSwitched";
                            //Console.WriteLine(_object.ToString()); 
                            //if (_guiConnector.Active)
                            //    _guiConnector.DeleteChannel(packets[1]);
                            break;
                        case "1":
                            Channel changedChannel = _parser.ParseChannel(data, true);
                            _clientUser.OnlineChannels[changedChannel.Name] = changedChannel;
                            if (OnChannelChangedLayout != null)
                                OnChannelChangedLayout(this, new ChannelChangedLayoutEventArgs(changedChannel));
                            _object = changedChannel;
                            _type = "ChannelChangedLayout";
                            break;
                        case "j":
                            if (packets[2] != "pics/-")
                                _clientUser.OnlineChannels[packets[1]].BackgroundImage = packets[3];
                            if (OnChannelBackGroundChanged != null)
                                OnChannelBackGroundChanged(this, new ChannelBackGroundChangedEventArgs(packets[2], packets[1]));
                            _object = new ChannelBackGroundChangedEventArgs(packets[2], packets[1]);
                            _type = "ChannelBackgroundChanged";
                            break;
                        //-------------------------------------------------------
                        //-----------------------Userbezogen---------------------
                        //-------------------------------------------------------
                        case "u":
                            UserList receivedUserList = _parser.ParseUserList(data);
                            _clientUser.OnlineChannels[receivedUserList.OwnerChannel].UserList = receivedUserList;
                            if (OnUserListReceived != null)
                                OnUserListReceived(this, new UserListReceivedEventArgs(receivedUserList));
                            _object = receivedUserList;
                            _type = "UserListReceived";
                            //if (_guiConnector.Active)
                            //    _guiConnector.AddUserList(_clientUser.OnlineChannels[receivedUserList.OwnerChannel]);
                            break;
                        case ".":
                            _clientUser.ByNames.AddRange(packets);
                            _clientUser.ByNames.RemoveAt(0);
                            _object = _clientUser.ByNames;
                            _type = "ByNamesReceived";
                            break;
                        case "l":

                            ChannelUser channelUser = _parser.ParseUser(data);
                            if (channelUser.ChannelJoined == null || channelUser.ChannelJoined == "-")
                                channelUser.ChannelJoined = _clientUser.SelectedChannel.Name;
                            _clientUser.OnlineChannels[channelUser.ChannelJoined].UserList.ChannelUserList.Add(channelUser);
                            if (OnUserJoinedChannel != null)
                                OnUserJoinedChannel(this, new UserJoinedChannelEventArgs(channelUser));
                            _object = channelUser;
                            _type = "UserJoinedChannel";
                            //if (_guiConnector.Active)
                            //    _guiConnector.AddUserToUserList(channelUser);

                            break;
                        case "w":

                            UserLeftChannel userLeftChannel = _parser.ParseUserLeftChannel(packets);
                            if (userLeftChannel.ChannelLeft == null || userLeftChannel.ChannelLeft == "-")
                                userLeftChannel.ChannelLeft = _clientUser.SelectedChannel.Name;
                            _clientUser.OnlineChannels[userLeftChannel.ChannelLeft].UserList.RemoveByName(userLeftChannel.Name);
                            if (OnUserLeftChannel != null)
                                OnUserLeftChannel(this, new UserLeftChannelEventArgs(userLeftChannel));
                            _object = userLeftChannel;
                            _type = "UserLeftChannel";
                            //if (_guiConnector.Active)
                            //    _guiConnector.RemoveUserFromUserList(userLeftChannel);
                            break;
                        case "m":
                            UserListImage userListImageToAdd = _parser.ParseUserListImage(packets);
                            _clientUser.OnlineChannels[userListImageToAdd.Channel].UserList.GetByName(userListImageToAdd.User).UserListImages.Add(userListImageToAdd.Image);
                            if (OnChangeUserListImage != null)
                                OnChangeUserListImage(this, new ChangeUserListImageEventArgs(userListImageToAdd, true));
                            _object = new ChangeUserListImageEventArgs(userListImageToAdd, true);
                            _type = "ChangeUserListImage";
                            break;
                        case "z":
                            UserListImage userListImageToRemove = _parser.ParseUserListImage(packets);
                            if (OnChangeUserListImage != null)
                                OnChangeUserListImage(this, new ChangeUserListImageEventArgs(userListImageToRemove, false));
                            _object = new ChangeUserListImageEventArgs(userListImageToRemove, false);
                            _type = "ChangeUserListImage";
                            break;
                        //-------------------------------------------------------
                        //------------------------Sonstiges----------------------
                        //-------------------------------------------------------
                        case "x":
                            if (OnOpenBrowserWindow != null)
                                OnOpenBrowserWindow(this, new OpenBrowserWindowEventArgs(packets[1]));
                            _object = packets[1];
                            _type = "OpenBrowserWindow";
                            break;
                        case "k":
                            if (!_loggedIn && (packets.Length >= 34 || data.Contains("Applet")))
                            {
                                LoginFailedEventArgs lfe = new LoginFailedEventArgs(LoginFailReason.Unknown);

                                if (data.Contains("Channellogin nicht möglich"))
                                    if (data.Contains("_Unsichtbare Channels_"))
                                        lfe = new LoginFailedEventArgs(LoginFailReason.ChannelDoesntExist);
                                    else
                                        lfe = new LoginFailedEventArgs(LoginFailReason.UserDoesntMeetChannelRestrictions);
                                else if (data.Contains("Der Channel"))
                                    lfe = new LoginFailedEventArgs(LoginFailReason.ChannelDoesntExist);
                                else if (data.Contains("Falsches Passwort"))
                                    lfe = new LoginFailedEventArgs(LoginFailReason.WrongUsernameOrPassword);
                                else if (data.Contains("Nick Gesperrt"))
                                {
                                    //Admin
                                    string admin = data.Substring(data.IndexOf("den Admin ") + 10);
                                    admin = admin.Substring(0, admin.IndexOf(" als Ansprechpartner per /m"));
                                    //Grund
                                    string reason = data.Substring(data.IndexOf("_:#") + 3);
                                    reason = reason.Substring(0, reason.IndexOf("##Bei Rückfragen"));
                                    //Sperrzeit
                                    string duration = "Unknown";
                                    if (data.Contains("_permanent gesperrt_"))
                                        duration = "Permanent";
                                    lfe = new LoginFailedEventArgs(LoginFailReason.UserLocked, reason, admin, duration);
                                }
                                else if (data.Contains("Applet"))
                                    lfe = new LoginFailedEventArgs(LoginFailReason.AppletTooOld);
                                else if (data.Contains("maximal"))
                                    lfe = new LoginFailedEventArgs(LoginFailReason.ChannelIsFull);
                                else if (data.Contains("Internetzugang") || data.Contains("Zugang gesperrt"))
                                    lfe = new LoginFailedEventArgs(LoginFailReason.IPLock);
                                if (OnLoginFailed != null)
                                    OnLoginFailed(this, lfe);
                                _object = lfe;
                                _type = "LoginFailed";
                            }

                            _object = new WindowOpenedEventArgs(packets[1].Replace("õf", ""), data, new HelperClasses.Parser.Popup.PopupParser(data));
                            if (OnWindowOpened != null)
                            {
                                OnWindowOpened(this, (WindowOpenedEventArgs)_object);
                            }
                            _type = "WindowOpened";
                            break;
                        case ":":
                            if (packets.Length > 2 && packets[2].Contains("PROTOCOL_HASH"))
                            {
                                Console.WriteLine("Empfange aktuelles Protokoll");
                                SetupModule(packets[2]);
                            }


                            if (_parentModule != null)
                            {
                                KnModule receivedModule = _parentModule.Parse(data);

                                if (OnChatComponentCommandReceived != null)
                                    OnChatComponentCommandReceived(this, new ChatComponentReceivedEventArgs(receivedModule));


                                switch (receivedModule.Name)
                                {
                                    case "MODULE_INIT":
                                        if (receivedModule.GetKeyValue().ContainsKey("serverPort"))
                                            if (OnCardServerConnectionEstablished != null)
                                                OnCardServerConnectionEstablished(this, new GameServerConnectionEventArgs(GameServer.GSClient.FromModule(receivedModule, this)));
                                        break;
                                }

                                _object = receivedModule;
                                _type = "ChatComponentReceived";
                            }
                            break;
                        //Noch nicht implementiert
                        case "q": _object = data; _type = "NotImplemented"; break;
                        case "+": _object = data; _type = "NotImplemented"; break;
                        case ";": _object = data; _type = "NotImplemented"; break;
                        case "6": _object = data; _type = "NotImplemented"; break;
                        case "!":
                            _clientUser.OnlineChannels.Remove(packets[1]);
                            if (_clientUser.SelectedChannel.Name == packets[1])
                                _clientUser.SelectedChannel = null;
                            if (_clientUser.OnlineChannels.Count < 1)
                                _loggedIn = false;
                            else
                                _clientUser.SelectedChannel = _clientUser.OnlineChannels.Last().Value;
                            break;
                        default:
                            Console.WriteLine(data);
                            break;
                    }
                    if (OnObjectFormed != null)
                        OnObjectFormed(this, new ObjectFormedEventArgs(_type, _object));
                }
                catch (WrongTokenException ex)
                {
                    if (OnGlobalException != null)
                        OnGlobalException(this, new GlobalExceptionEventArgs(ex, KDFModule.Parsing));
                }
                catch (Exception ex)
                {
                    if (OnGlobalException != null) OnGlobalException(this, new GlobalExceptionEventArgs(ex, KDFModule.Client));
                }
        }
        #endregion

        #region Module
        private void SetupModule(string protocolConfig)
        {
            _parentModule = KnModule.StartUp(protocolConfig);
        }

        private void UpdateModule()
        {
            KnModule start = KnModule.StartUp(global::KDF.Properties.Resources.module);
            KnModule request = start.CreateModule("CONFIRM_PROTOCOL_HASH");
            request.Add("PROTOCOL_HASH", long.Parse(start.Hash));
            this.Send("q\0" + Encoding.Default.GetString(start.WriteBytes(request)));
        }
        #endregion

        #region Send
        /// <summary>
        /// Sendet ein Paket zum Server
        /// </summary>
        /// <param name="data">Das Paket was zum Server gesendet werden soll.</param>
        public void Send(string data)
        {
            if (Connected && data.StartsWith("n"))
            {
                string[] lParams = data.Split(_nullChar);
                _connection.Login(lParams[1], lParams[2], lParams[3]);
            }
            else if (Connected)
                _connection.Send(data, 0x00);
        }

        /// <summary>
        /// Sendet eine öffentliche Nachricht an den Server
        /// </summary>
        /// <param name="publicMessage">Gibt die öffentliche Nachricht an, die gesendet werden soll</param>        
        public void SendUserInput(PublicMessage publicMessage)
        {
            Send(string.Format("e{0}{1}{0}{2}", _nullChar, publicMessage.Channel, publicMessage.Message));
        }

        /// <summary>
        /// Sendet eine private Nachricht
        /// </summary>
        /// <param name="privateMessage">Gibt die private Nachricht an, die gesendet werden soll</param>        
        public void SendUserInput(PrivateMessage privateMessage)
        {
            Send(string.Format("e{0}{1}{0}/p {2}:{3}", _nullChar, privateMessage.FromChannel, string.Join(",", privateMessage.Receivers), privateMessage.Message));
        }

        /// <summary>
        /// Sendet eine öffentliche Nachricht an den Server
        /// </summary>
        /// <param name="message">Gibt den Text an der an den Server gesendet werden soll</param>
        public void SendUserInput(string message)
        {
            try
            {
                if (_clientUser.SelectedChannel != null)
                    SendUserInput(_clientUser.SelectedChannel.Name, message);
            }
            catch  { }
        }

        /// <summary>
        /// Sendet eine öffentliche Nachricht an den Server
        /// </summary>
        /// <param name="channel">Gibt den Channel an, an welchen die Nachricht gesendet werden soll</param>
        /// <param name="message">Gibt den Text an, der an den angegebenen Channel gesendet werden soll</param>       
        public void SendUserInput(string channel, string message)
        {
            SendUserInput(new PublicMessage(null, channel, message));
        }

        /// <summary>
        /// Sendet dem Server die Information, dass man einen Channel verlassen hat
        /// </summary>
        /// <param name="channelName">Gibt den Namen des Channels an, der verlassen werden soll</param>
        /// <param name="OnConnectionStateChanged">Gibt das Event an, welches ausgelöst werden soll, sobald der Client sich in keinem Channel mehr befindet</param>
        /// <remarks>Wird ein Channelname angegeben, in welchem sich der Client nicht eingeloggt hat, wird nichts unternommen</remarks>        
        public void LeaveChannel(string channelName, EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged)
        {
            if (ClientUser.OnlineChannels.ContainsKey(channelName))
            {
                if (_guiConnector.Active)
                    _guiConnector.DeleteChannel(channelName);

                Send(string.Format("w{0}{1}", _nullChar, channelName));
                ClientUser.OnlineChannels.Remove(channelName);
                if (ClientUser.OnlineChannels.Count == 0 && OnConnectionStateChanged != null)
                    OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, true));
            }
        }
        #endregion

        #region Connect
        /// <summary>
        /// Verbindet sich zum angegebenen Chatsystem.
        /// </summary>
        /// <param name="chatSystem">Chatsystem zu dem der Client sich verbinden soll.</param>
        public void Connect(ChatSystem chatSystem)
        {
            _serverHost = chatSystem == ChatSystem.com ? "knuddels.com" : "knuddels.net";
            _serverPort = (int)chatSystem;

            _connection.ChatSystem = chatSystem;
            _connection.UseIPhone = _clientType == ClientType.iPhone;
            _connection.Connect();
        }

        /// <summary>
        /// Verbindet sich zum angegebenen Chatsystem und wählt die angegebene Kategorie für die Channelauflistung
        /// </summary>
        /// <param name="chatSystem">Chatsystem zu dem der Client sich verbinden soll.</param>
        /// <param name="category">Die Channelkategorie die beim Verbinden abgerufen werden soll.</param>
        public void Connect(ChatSystem chatSystem, int category)
        {
            _serverHost = chatSystem == ChatSystem.com ? "knuddels.com" : "knuddels.net";
            _serverPort = (int)chatSystem;

            _connection.UseIPhone = _clientType == ClientType.iPhone;
            _connection.Category = category;
            _connection.ChatSystem = chatSystem;
            _connection.Connect();
        }

        /// <summary>
        /// Verbindet sich zum angegebenen Chatsystem und benutzt dabei einen Socks5-Proxy
        /// </summary>
        /// <param name="chatSystem">Chatsystem zu dem der Client sich verbinden soll.</param>
        /// <param name="proxyIP">Die IP-Adresse des Proxys</param>
        /// <param name="proxyPort">Der Port des Proxys</param>
        public void Connect(ChatSystem chatSystem, int proxyPort, string proxyIP)
        {
            _serverHost = chatSystem == ChatSystem.com ? "knuddels.com" : "knuddels.net";
            _serverPort = (int)chatSystem;

            _connection.UseIPhone = _clientType == ClientType.iPhone;
            _connection.ChatSystem = chatSystem;
            _connection.ProxyPort = proxyPort;
            _connection.ProxyIp = proxyIP;
            _connection.Connect();
        }

        /// <summary>
        /// Verbindet sich zum angegebenen Chatsystem und benutzt dabei einen Socks5-Proxy
        /// </summary>
        /// <param name="chatSystem">Chatsystem zu dem der Client sich verbinden soll.</param>
        /// <param name="proxyIP">Die IP-Adresse des Proxys</param>
        /// <param name="proxyPort">Der Port des Proxys</param>
        /// <param name="category">Die Channelkategorie die beim Verbinden abgerufen werden soll.</param>
        public void Connect(ChatSystem chatSystem, int proxyPort, string proxyIP, int category)
        {
            _serverHost = chatSystem == ChatSystem.com ? "knuddels.com" : "knuddels.net";
            _serverPort = (int)chatSystem;

            _connection.UseIPhone = _clientType == ClientType.iPhone;
            _connection.Category = category;
            _connection.ChatSystem = chatSystem;
            _connection.ProxyPort = proxyPort;
            _connection.ProxyIp = proxyIP;
            _connection.Connect();
        }
        #endregion

        #region Login
        /// <summary>
        /// Sendet ein Login-Token an den Server
        /// </summary>
        /// <param name="username">Der Nickname der eingeloggt werden soll</param>
        /// <param name="password">Das Passwort des Nicknamens der eingeloggt werden soll</param>
        /// <param name="channel">Der Channel der betreten werden soll</param>
        /// <remarks>Da beim Login über das Applet auf den Websiten von Knuddels auch immer ein Channel angegeben werden muss,
        /// ist das bei der Verwendung eines Clienthacks genauso. Der Channel kann natürlich später gewechselt werde, oder es können
        /// bis zu 4 Channels gleichzeitig betreten werden.</remarks>
        public void Login(string username, string password, string channel)
        {
            ClientUser = new ClientUser(username, password);
            Send(string.Format("n{0}{1}{0}{2}{0}{3}", _nullChar, channel, username, password));
        }

        /// <summary>
        /// Sendet ein Login-Token an den Server
        /// </summary>
        /// <param name="username">Der Nickname der eingeloggt werden soll</param>
        /// <param name="password">Das Passwort des Nicknamens der eingeloggt werden soll</param>
        /// <param name="channel">Der Channel der betreten werden soll</param>
        /// <param name="securityActive">Gibt an ob die Security verwendet werden soll (AntiAdmin, AntiNRS, AntiBotcheck), Wichtig: Kann nur einmal eingestellt werden! </param>
        /// <param name="showSecurityGUI">Gibt an ob die GUI Elemente bei Securityaktivitäten angezeigt werden soll (z.B. /m-Fenster), Wichtig: Für mehr Sicherheit (true) übergeben, Kann nur einmal eingestellt werden!</param>
        /// <remarks>Da beim Login über das Applet auf den Websiten von Knuddels auch immer ein Channel angegeben werden muss,
        /// ist das bei der Verwendung eines Clienthacks genauso. Der Channel kann natürlich später gewechselt werde, oder es können
        /// bis zu 4 Channels gleichzeitig betreten werden.</remarks>
        public void Login(string username, string password, string channel, bool securityActive, bool showSecurityGUI)
        {
            if (!_securityWindowIsOpen)
            {
                new Thread(delegate() { System.Windows.Forms.Application.Run(new Graphics.SecurityWindow.CommunicationWindow(this, securityActive, showSecurityGUI)); }).Start();
                _securityWindowIsOpen = true;
            }
            ClientUser = new ClientUser(username, password);
            Send(string.Format("n{0}{1}{0}{2}{0}{3}", _nullChar, channel, username, password));
        }
        #endregion

        public void UseKDFGui(ExtendedInput extendedInput, UserListControl userListControl, HTMLChatHistory htmlChatHistory)
        {
            _guiConnector = new GuiConnector(extendedInput, userListControl, htmlChatHistory, this);
        }

        /// <summary>
        /// Beendet die Verbindung zum Server mit dem der Client verbunden ist
        /// </summary>
        public void Disconnect()
        {
            if (this.Connected)
            {
                _connection.Disconnect();
                _cardServerClients.Values.ToList().ForEach(client =>
                {
                    if (client != null && client.Connected)
                        client.Disconnect();
                });
                _cardServerClients.Clear();

                this._loggedIn = false;
                //_connection = new Connection(this, ref OnConnectionStateChanged, ref OnGlobalException);
            }
        }
        /// <summary>
        /// Dient dazu softwareseitig den Channel zu wechseln, zu welchem die öffentlichen Nachrichten gesendet werden
        /// </summary>
        /// <param name="channelName">Der Channel, in welchen zukünftig die öffentlichen Nachrichten gesendet werden sollen</param>
        /// <remarks>Bei einem Channeljoin wird der Wert automatisch auf den Channel gesetzt, in welchen gejoint wurde</remarks>
        public void ChangeSelectedChannel(string channelName)
        {
            if (!_clientUser.OnlineChannels.ContainsKey(channelName))
                return;

            //if (_guiConnector.Active)
            //    _guiConnector.SelectChannel(channelName);

            _clientUser.SelectedChannel = _clientUser.OnlineChannels[channelName];
        }
    }
}
