using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnLoggedInStateChanged-Ereignis bereit
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        private bool _loggedIn;
        /// <summary>
        /// Ruft ab, ob der Client eingeloggt ist oder nicht.
        /// </summary>
        public bool LoggedIn { get { return _loggedIn; } }

        private bool _connected;
        /// <summary>
        /// Ruft ab, ob der Client mit dem Chatserver verbunden ist, oder nicht.
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        /// <summary>
        /// Ruft den Grund ab, falls die Verbindung vom Remote Host gesteuert abgebrochen wurde
        /// </summary>
        private string _reason;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ChannelLeftEventargs-Klasse.
        /// </summary>
        /// <param name="loggedIn">Gibt an ob der Client eingeloggt ist oder nicht.</param>
        /// <param name="connected">Gibt an ob der Client mit dem Chatserver verbunden ist oder nicht.</param>
        public ConnectionStateChangedEventArgs(bool loggedIn, bool connected)
        {
            _loggedIn = loggedIn;
            _connected = connected;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ChannelLeftEventargs-Klasse.
        /// </summary>
        /// <param name="loggedIn">Gibt an ob der Client eingeloggt ist oder nicht.</param>
        /// <param name="connected">Gibt an ob der Client mit dem Chatserver verbunden ist oder nicht.</param>
        public ConnectionStateChangedEventArgs(bool loggedIn, bool connected, string reason)
        {
            _loggedIn = loggedIn;
            _connected = connected;
            _reason = reason;
        }
    }
}
