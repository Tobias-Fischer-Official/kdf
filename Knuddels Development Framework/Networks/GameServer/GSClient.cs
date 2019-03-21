using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDF.Networks.Protocol.Module;
using KDF.Networks.Core;
using System.Collections;
using System.Net.Sockets;
using KDF.ChatObjects;
using KDF.ClientEventArgs;
using System.Threading;
using KDF.Networks.Protocol;
using System.IO;
using System.Windows.Forms;
using System.Windows;

namespace KDF.Networks.GameServer
{
    /// <summary>
    /// Die vorhandenen CardServer
    /// </summary>
    public enum GameServer : int
    {
        /// <summary>
        /// CardServer für Poker in DE
        /// </summary>
        Poker_DE = 2810,
        /// <summary>
        /// CardServer für MauMau in DE
        /// </summary>
        MauMau_DE = 2910,
        /// <summary>
        /// CardServer für SmileyWars in DE
        /// </summary>
        SmileyWars_DE = 3810
    }

    /// <summary>
    /// Der CardServer Client um sich z.b bei Poker anzumelden
    /// </summary>
    /// <emaple>
    /// <code>
    /// using System;
    /// using KDF.Networks.Core;
    /// using KDF.Networks.GameServer;
    ///
    ///namespace KDF_Console_Sample
    ///{
    ///    class Program
    ///    {
    ///        static void Main(string[] args)
    ///        {
    ///            KnuddelsClient c = new KnuddelsClient();
    ///            
    ///            c.OnConnectionStateChanged += new EventHandler&lt;KDF.ClientEventArgs.ConnectionStateChangedEventArgs&gt;(c_OnConnectionStateChanged);
    ///            c.OnCardServerConnection += new EventHandler&lt;KDF.ClientEventArgs.GameServerConnectionEventArgs&gt;(c_OnCardServerConnection);
    ///
    ///            c.Connect(ChatSystem.de);
    ///        }
    ///        
    ///        static void c_OnConnectionStateChanged(object sender, KDF.ClientEventArgs.ConnectionStateChangedEventArgs e)
    ///        {
    ///            if (e.Connected && !e.LoggedIn)
    ///                ((KnuddelsClient)sender).Login("nickname", "password", "channel");
    ///        }
    ///        
    ///        static void c_OnCardServerConnection(object sender, KDF.ClientEventArgs.CardServerConnectionEventArgs e)
    ///        {
    ///            GSClient client = e.Client;
    ///            
    ///            client.OnConnectionStateChanged += new EventHandler&lt;KDF.ClientEventArgs.ConnectionStateChangedEventArgs&gt;(cs_OnConnectionStateChanged);
    ///            client.OnModuleReceived += new EventHandler&lt;KDF.ClientEventArgs.ModuleReceivedEventArgs&gt;(cs_OnModuleReceived);
    ///            
    ///            client.Connect();
    ///        }
    ///        
    ///        static void cs_OnConnectionStateChanged(object sender, KDF.ClientEventArgs.ConnectionStateChangedEventArgs e)
    ///        {
    ///            GSClient client = (CSClient)sender;
    ///            if (e.Connected && !e.LoggedIn)
    ///                client.Login();
    ///            else if (e.Connected && e.LoggedIn)
    ///                client.JoinRomm();
    ///        }
    ///        
    //         static void cs_OnConnectionStateChanged(object sender, KDF.ClientEventArgs.ConnectionStateChangedEventArgs e)
    ///        {
    ///             Console.WriteLine("[CardServer] " + e.Module.Name);
    ///        }
    ///    }
    ///}
    /// </code>
    /// </example>
    public class GSClient
    {
        #region Private
        private NetworkStream _knDataStream;
        private Socket _knSocket;
        #endregion

        #region EventHandler
        internal event EventHandler<GlobalExceptionEventArgs> _onGlobalException;

        /// <summary>
        /// Das Event welches ausgelöst wird, wenn sich der Zustand des Logins ändert
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn eine Instanz der KnModule Klasse empfangen wurde
        /// </summary>
        public event EventHandler<ModuleReceivedEventArgs> OnModuleReceived;
        #endregion

        #region Public
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

        private string _channel = string.Empty;
        /// <summary>
        /// Gibt den aktuellen Channel des Clients aus
        /// </summary>
        public string Channel
        {
            get { return _channel; }
        }

        private string _gameId = string.Empty;
        /// <summary>
        /// Gibt die aktuelle GameId des Clients aus
        /// </summary>
        public string GameId
        {
            get { return _gameId; }
        }

        private ClientUser _clientUser;
        /// <summary>
        /// Ruft den aktuellen User des Clients ab
        /// </summary>
        public ClientUser ClientUser
        {
            get { return _clientUser; }
            set { _clientUser = value; }
        }

        private KnuddelsClient _client;
        /// <summary>
        /// Gibt den KnuddelsClient zurück über dem die Cardserver Verbindung gestartet wurde
        /// </summary>
        public KnuddelsClient Client
        {
            get { return _client; }
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

        private bool _connected = false;
        /// <summary>
        /// Ruft ab ob der Client mit dem Cardserver verbunden ist
        /// </summary>
        public bool Connected
        {
            get { return (_connected && _knSocket != null && _knSocket.Connected); }
        }

        private bool _loggedIn = false;
        /// <summary>
        /// Gibt an ob der Client mit einem Nicknamen und Passwort an dem Cardserver, mit welchem er verbunden ist, angemeldet ist
        /// </summary>
        public bool LoggedIn
        {
            get { return _loggedIn; }
        }

        private KnModule _parentModule;
        /// <summary>
        /// Gibt eine Instanz der Klasse KnModule zurück, mit dieser können neue Instanzen erstellt werden
        /// </summary>
        public KnModule ParentModule
        {
            get { return _parentModule; }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Erstellt eine neue Instanz des Clients
        /// </summary>
        public GSClient()
        {
            _parentModule = KnModule.StartUp("1686426172;20;PROTOCOL_HASH;CONFIRM_PROTOCOL_HASH;PROTOCOL_CONFIRMED;PROTOCOL_DATA;CHANGE_PROTOCOL;;5;;20;;;9;;23;;:;;");
        }
        #endregion

        #region Connect
        /// <summary>
        /// Verbindet sich zum von Knuddels angebenen CardServer
        /// </summary>
        /// <remarks>Bitte diese Funktion nur nutzen wenn der Client vom KDF erstellt wurde</remarks>
        public void Connect()
        {
            Connect(_serverHost, _serverPort);
        }

        /// <summary>
        /// Verbindet sich zum angegebenen CardServer
        /// </summary>
        /// <param name="pCardServer">Der CardServer zu dem der Client sich verbinden soll</param>
        public void Connect(GameServer pCardServer)
        {
            Connect(pCardServer, null, 0);
        }

        /// <summary>
        /// Verbindet sich zum angegebenen CardServer und verwendet dabei einen Socks5-Proxy
        /// </summary>
        /// <param name="pCardServer">Der CardServer zu dem der Client sich verbinden soll</param>
        /// <param name="pProxyHost">Die IP-Adresse des Proxys</param>
        /// <param name="pProxyPort">Der Port des Proxys</param>
        public void Connect(GameServer pCardServer, string pProxyHost, int pProxyPort)
        {
            Connect("knuddels.net", (int)pCardServer, pProxyHost, pProxyPort);
        }

        /// <summary>
        /// Verbindet sich zum angebenen Server
        /// </summary>
        /// <param name="pHost">Die IP-Adresse des Servers</param>
        /// <param name="pPort">Der Port des Servers</param>
        public void Connect(string pHost, int pPort)
        {
            Connect(pHost, pPort, null, 0);
        }

        /// <summary>
        /// Verbindet sich zum angebenen Server und verwendet dabei einen Socks5-Proxy
        /// </summary>
        /// <param name="pHost">Die IP-Adresse des Servers</param>
        /// <param name="pPort">Der Port des Servers</param>
        /// <param name="pProxyHost">Die IP-Adresse des Proxys</param>
        /// <param name="pProxyPort">Der Port des Proxys</param>
        public void Connect(string pHost, int pPort, string pProxyHost, int pProxyPort)
        {
            try
            {
                _knSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                if (pProxyHost != null && pProxyPort != 0)
                {
                    _knSocket.ReceiveTimeout = 30000;
                    _knSocket.SendTimeout = 30000;
                    _knSocket.Connect(pProxyHost, pProxyPort);
                    SocksProxy.EstablishConnection(_knSocket, pHost, pPort);
                }
                else
                {
                    _knSocket.Connect(pHost, pPort);
                    _knDataStream = new NetworkStream(_knSocket);
                }

                Thread thr = new Thread(Receive);
                thr.Start();
            }
            catch (Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
            }
        }
        #endregion

        #region Disconnect
        /// <summary>
        /// Beendet die Verbindung zum Server mit dem der Client verbunden ist
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_knSocket.Connected)
                {
                    if (LoggedIn)
                        LeaveRoom();
                    _knSocket.Disconnect(false);
                }
            }
            catch (SocketException ex)
            {
                string s = ex.ToString();
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Sendet eine Instanz der KnModule an den Server
        /// </summary>
        /// <param name="pModule">Die Instanz was zum Server gesendet werden soll.</param>
        public void Send(KnModule pModule)
        {
            try
            {
                byte[] arrayOfByte1 = _parentModule.WriteBytes(pModule);
                byte[] arrayOfByte2 = encode(arrayOfByte1.Length);
                byte[] arrayOfByte3 = new byte[arrayOfByte2.Length + arrayOfByte1.Length];
                Array.Copy(arrayOfByte2, 0, arrayOfByte3, 0, arrayOfByte2.Length);
                Array.Copy(arrayOfByte1, 0, arrayOfByte3, arrayOfByte2.Length, arrayOfByte1.Length);
                if (arrayOfByte3.Length == 0)
                    throw new IOException("z9.a(21815)");
                else
                    _knDataStream.Write(arrayOfByte3, 0, arrayOfByte3.Length);
            }
            catch
            {
                Disconnect();
            }
        }
        #endregion

        #region Login/Leave/Join Room

        /// <summary>
        /// Sendet eine Login-Instanz an den Server
        /// </summary>
        /// <remarks>Bitte diese Funktion nur nutzen wenn der Client vom KDF erstellt wurde</remarks>
        public void Login()
        {
            Login(_clientUser.Username, _clientUser.Password);
        }

        /// <summary>
        /// Sendet eine Login-Instanz an den Server
        /// </summary>
        /// <param name="pUsername">Der Username der für den Login genutzer werden soll</param>
        /// <param name="pPassword">Das Passwort das für den Login genutzer werden soll</param>
        public void Login(string pUsername, string pPassword)
        {
            KnModule login = _parentModule.CreateModule("LOGIN");
            login.Add("USER_NAME", pUsername);
            login.Add("PASSWORD", pPassword);
            Send(login);
        }

        /// <summary>
        /// Verlässt den Raum
        /// </summary>
        /// <remarks>Bitte diese Funktion nur verwenden wenn der Client vom KDF erstellt wurde</remarks>
        public void LeaveRoom()
        {
            LeaveRoom(_gameId);
        }

        /// <summary>
        /// Verlässt den Raum
        /// </summary>
        /// <param name="pGameId">Dei GameID des Raum's</param>
        public void LeaveRoom(string pGameId)
        {
            KnModule leaveRoom = _parentModule.CreateModule("LEAVE_ROOM");
            leaveRoom.Add("GAME_ID", pGameId);
            Send(leaveRoom);
        }

        /// <summary>
        /// Betritt einen Raum
        /// </summary>
        /// <remarks>Bitte diese Funktion nur nutzen wenn der Client vom KDF erstellt wurde</remarks>
        public void JoinRoom()
        {
            JoinRoom(_gameId);
        }

        /// <summary>
        /// Betritt einen Raum
        /// </summary>
        /// <param name="pGameId">Die GameID des Raum's</param>
        public void JoinRoom(string pGameId)
        {
            KnModule joinRoom = _parentModule.CreateModule("JOIN_ROOM");
            joinRoom.Add("GAME_ID", pGameId);
            Send(joinRoom);
        }
        #endregion

        #region Encode/Decode
        private byte[] encode(int paramInt)
        {
            if (paramInt == 0)
                return new byte[0];
            byte[] arrayOfByte = (byte[])null;
            paramInt--;
            if (paramInt >= 0)
                if (paramInt < 128)
                {
                    arrayOfByte = new byte[1];
                    arrayOfByte[0] = (byte)paramInt;
                }
                else
                {
                    int i1;
                    for (i1 = 0; 32 << (i1 + 1 << 3) <= paramInt; i1 = (byte)(i1 + 1)) ;
                    arrayOfByte = new byte[i1 + 2];
                    byte tmp73_72 = (byte)(i1 + 1);
                    i1 = tmp73_72;
                    arrayOfByte[0] = (byte)(tmp73_72 << 5 | 0x80 | paramInt & 0x1F);
                    for (int i2 = 1; i2 < arrayOfByte.Length; i2++)
                        arrayOfByte[i2] = (byte)(paramInt >> 8 * (i2 - 1) + 5);
                }
            return arrayOfByte;
        }

        private byte[] decode(Stream stream)
        {
            try
            {
                sbyte first = (sbyte)stream.ReadByte();

                if (first == -1)
                {
                    if (OnConnectionStateChanged != null)
                        OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, false));
                    return null;
                    //throw new IOException("End of stream");
                }
                else
                {

                    int length;

                    if (first >= 0)
                    {
                        length = first + 1;
                    }
                    else
                    {
                        length = (first & 0x1F) + 1;
                        int count = ((first & 0x60) >> 5);

                        for (int i = 0; i < count; i++)
                        {
                            length += (stream.ReadByte() << (i << 3) + 5);
                        }
                    }
                    byte[] buffer = new byte[length];

                    for (int i = 0; i < length; i++)
                    {
                        buffer[i] = (byte)stream.ReadByte();
                    }
                    return buffer;
                }

            }
            catch
            {
                //if (_onGlobalException != null)
                //    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
                return null;
            }
        }
        #endregion

        #region Receive
        private void Receive()
        {
            KnModule confirmProtocolHash = _parentModule.CreateModule("CONFIRM_PROTOCOL_HASH");
            confirmProtocolHash.Add("PROTOCOL_HASH", long.Parse(_parentModule.Hash));
            Send(confirmProtocolHash);

            while (_knSocket.Connected)
            {
                try
                {
                    KnModule received = _parentModule.Parse(decode(_knDataStream));
                    if (received != null)
                    {
                        if (_formToInvoke != null)
                        {
                            if (_formToInvoke.InvokeRequired)
                                _formToInvoke.Invoke((MethodInvoker)delegate
                                {
                                    HandleIncomingData(received);
                                });
                        }
                        else if (_windowToInvoke != null)
                        {
                            _windowToInvoke.Dispatcher.Invoke((MethodInvoker)delegate
                            {
                                HandleIncomingData(received);
                            });
                        }
                        else
                            HandleIncomingData(received);
                        _connected = true;
                    }
                }
                catch (Exception ex)
                {
                    _loggedIn = false;
                    
                    if (OnConnectionStateChanged != null)
                        if (_formToInvoke != null)
                        {
                            if (_formToInvoke.InvokeRequired)
                                _formToInvoke.Invoke((MethodInvoker)delegate
                                {
                                    OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, false));
                                });
                        }
                        else if (_windowToInvoke != null)
                        {
                            _windowToInvoke.Dispatcher.Invoke((MethodInvoker)delegate
                            {
                                OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, false));
                            });
                        }
                        else
                            OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, false));
                    _client.CardServerClients.Remove(_channel);
                    if (_onGlobalException != null)
                        _onGlobalException(this, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                    Disconnect();
                }
            }
        }
        #endregion

        #region Handle
        private void HandleIncomingData(KnModule data)
        {
            switch (data.Name)
            {
                case "CHANGE_PROTOCOL":
                case "PROTOCOL_CONFIRMED":
                    if (data.Name.Equals("CHANGE_PROTOCOL"))
                        _parentModule.ParseTree(data.GetValue<string>("PROTOCOL_DATA"));

                    if (OnConnectionStateChanged != null)
                        OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, true));
                    break;
                case "LOGGIN_PROCESSED":
                    if (!_loggedIn)
                    {
                        if (data.GetValue<byte>("LOGGIN_RESULT") == _parentModule.GetValue("LOGGIN_RESULT", "LOGGED_IN"))
                        {
                            _loggedIn = true;
                            if (OnConnectionStateChanged != null)
                                OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(true, true));
                        }
                        else
                        {
                            if (OnConnectionStateChanged != null)
                                OnConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, true));
                        }
                    }
                    break;
            }

            if (OnModuleReceived != null)
                OnModuleReceived(this, new ModuleReceivedEventArgs(data));
        }
        #endregion

        internal static GSClient FromModule(KnModule pModule, KnuddelsClient pClient)
        {
            Dictionary<string, object> keyValueDic = pModule.GetKeyValue();
            GSClient client = new GSClient()
                {
                    _channel = pModule.GetValue<string>("CHANNEL_NAME"),
                    _client = pClient,
                    _gameId = keyValueDic["gameId"].ToString(),
                    _clientUser = new ClientUser(pClient.ClientUser.Username, keyValueDic["oneTimePassword"].ToString()),
                    _serverHost = pClient.ServerHost,
                    _serverPort = int.Parse(keyValueDic["serverPort"].ToString()),
                    _formToInvoke = pClient.FormToInvoke,
                    _windowToInvoke = pClient.WindowToInvoke
                };
            pClient.CardServerClients.Add(client.Channel, client);
            return client;
        }

    }
}
