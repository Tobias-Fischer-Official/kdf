using System;
using System.Text;
using System.IO;
using KDF.Networks.Protocol;
using System.Net.Sockets;
using System.Threading;
using KDF.Properties;
using KDF.ChatObjects;
using KDF.ClientEventArgs;
using System.Reflection;
using System.Diagnostics;
using KDF.Networks.Core;

namespace KDF.Networks.Core
{
    internal class Connection
    {
        private NetworkStream _knDataStream;
        private Socket _knSocket;
        public Socket KnSocket
        {
            get { return _knSocket; }
            set { _knSocket = value; }
        }
        
        private Socket _kdfSocket;

        private bool firstmessage = true;
        private byte[] _decodeKey = null;

        private Compress compress;
        private Decompress decompress;
        private KnuddelsClient _client;

        private string _version;
        private char _encryptKey;
        private string _knKey;

        private string _username;
        private string _pw;
        private string _channel;

        private ChatSystem _chatSystem;
        public ChatSystem ChatSystem
        {
            get { return _chatSystem; }
            set { _chatSystem = value; }
        }

        private int _proxyPort = 0;
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        private string _proxyIp;
        public string ProxyIp
        {
            get { return _proxyIp; }
            set { _proxyIp = value; }
        }

        private int _category = 0;
        public int Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private bool _useIPhone = false;
        public bool UseIPhone
        {
            get { return _useIPhone; }
            set { _useIPhone = value; }
        }

        internal event EventHandler<ConnectionStateChangedEventArgs> _onConnectionStateChanged;
        internal event EventHandler<GlobalExceptionEventArgs> _onGlobalException;
        private bool _connected = false;

        internal bool Connected
        {
            get { return (_connected && _knSocket != null && _knSocket.Connected); }
        }

        internal Connection(KnuddelsClient client)
        {
            
            _client = client;
            string huffmantree = Encoding.UTF8.GetString(Resources.huffmantree);
            compress = new Compress(huffmantree);
            decompress = new Decompress(huffmantree);
        }
        internal void Disconnect()
        {
            try
            {
                if (_knSocket.Connected)
                {
                    Send("d", 0);
                    _knSocket.Disconnect(false);
                }
            }
            catch (SocketException ex)
            {
                string s = ex.ToString();
            }
        }

        private void Receive()
        {
            while (_knSocket.Connected)
            {
                try
                {
                    string received = decompress.Run(decode(_knDataStream));
                    if (received != null)
                    {
                        if (received.StartsWith("("))
                        {
                            _decodeKey = Encoding.UTF8.GetBytes(received.Split('\0')[3]);
                            _knKey = received.Split('\0')[1];
                        }
                        _connected = true;
                        _client.HandleIncomingData(received);
                    }
                }
                catch (Exception ex)
                {
                    if (_onGlobalException != null)
                        _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                    Disconnect();
                }
            }
        }

        internal void Login(string channel, string username, string password)
        {
            try
            {
                _client.ClientUser = new ClientUser(username, password);
                _channel = channel;
                _username = username;
                _pw = password;

                Thread thr = new Thread(loginWithPwHash);
                thr.Start();
            }
            catch (Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
            }
        }

        private void loginWithPwHash()
        {
            try
            {
                if (!_kdfSocket.Connected)
                {
                    if (_onGlobalException != null)
                        _onGlobalException(this._client, new GlobalExceptionEventArgs(new Exception("KDFServer Disconnected"), KDFModule.HashServer));
                    Disconnect();
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _pw.Length; i++)
                    sb.Append((char)(_pw[i] ^ (int)_encryptKey));
                string toSend = sb.ToString() + "\0" + _knKey;
                byte[] sbuffer = Encoding.UTF8.GetBytes(toSend);
                _kdfSocket.Send(sbuffer);
                sbuffer = new byte[100];

                _kdfSocket.Receive(sbuffer);
                string pwhash = Encoding.UTF8.GetString(sbuffer).TrimEnd('\0');
                Send("n\0" + _channel + "\0" + _username + "\0" + pwhash + "\0F", 0x00);
            }
            catch (Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
            }
        }

        internal void Connect()
        {
            try
            {
                _kdfSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _knSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                string server = _chatSystem == ChatSystem.com ? "knuddels.com" : "knuddels.net";

                if (_proxyIp != null && _proxyPort != 0)
                {
                    _knSocket.ReceiveTimeout = 30000;
                    _knSocket.SendTimeout = 30000;
                    if (_kdfSocket != null)
                    {
                        _kdfSocket.ReceiveTimeout = 30000;
                        _kdfSocket.SendTimeout = 30000;
                    }
                    _knSocket.Connect(_proxyIp, _proxyPort);
                    SocksProxy.EstablishConnection(_knSocket, server, (int)_chatSystem);
                }
                else
                {
                    _knSocket.Connect(server, (int)_chatSystem);

                }
                _knDataStream = new NetworkStream(_knSocket);
                try
                {
                    _kdfSocket.Connect("192.168.2.110", 2711);                    
                }
                catch
                {
                    _kdfSocket.Connect("spin-solutions.dyndns.org", 2711);
                }
                _kdfSocket.Send(Encoding.UTF8.GetBytes(_chatSystem.ToString()));
                byte[] buffer = new byte[100];
                _kdfSocket.Receive(buffer);

                string[] v = Encoding.UTF8.GetString(buffer).TrimEnd('\0').Split('\0');
                _version = v[0];
                _encryptKey = v[1][0];
                if (!_useIPhone)
                {
                    Send("t\0" + _version + " \0http://www.knuddels.de/index.html?c=0\0" + _category + "\01.6.0_22\0-\046513\0Java HotSpot(TM) Server VM\0-", 0);
                }
                else
                {
                    char[] hexChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
                    Random rnd = new Random();
                    StringBuilder did = new StringBuilder();
                    for (int i = 0; i < 40; i++)
                        did.Append(hexChars[rnd.Next(0, hexChars.Length)]);

                    Send("t\0" + _version + "\00\011\0iphone\00\00\0iPhone\0iphone" + did.ToString(), 0);
                }
                Thread thr = new Thread(Receive);
                thr.Start();
            }
            catch (Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
            }
        }

        internal void Send(string input, int paramInt)
        {
            try
            {
                int i1 = firstmessage ? 1 : 0;
                byte[] arrayOfByte1 = compress.Run(input, paramInt);
                byte[] arrayOfByte2 = encode(arrayOfByte1.Length);
                byte[] arrayOfByte3 = new byte[arrayOfByte2.Length + arrayOfByte1.Length + i1];
                Array.Copy(arrayOfByte2, 0, arrayOfByte3, i1, arrayOfByte2.Length);
                Array.Copy(arrayOfByte1, 0, arrayOfByte3, arrayOfByte2.Length + i1, arrayOfByte1.Length);
                if (arrayOfByte3.Length == 0)
                    throw new IOException("z9.a(21815)");
                else
                    _knDataStream.Write(arrayOfByte3, 0, arrayOfByte3.Length);
                firstmessage = false;
            }
            catch (Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
            }
        }

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
                    if (_onConnectionStateChanged != null)
                        _onConnectionStateChanged(this, new ConnectionStateChangedEventArgs(false, false));
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
                        buffer[i] = (byte)((byte)stream.ReadByte() ^ (_decodeKey != null && i < _decodeKey.Length ? _decodeKey[i] : 0));
                    }
                    return buffer;
                }

            }
            catch(Exception ex)
            {
                if (_onGlobalException != null)
                    _onGlobalException(this._client, new GlobalExceptionEventArgs(ex, KDFModule.Networks));
                Disconnect();
                return null;
            }
        }
    }
}
