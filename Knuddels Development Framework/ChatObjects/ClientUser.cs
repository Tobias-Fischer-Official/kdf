using System.Collections.Generic;

namespace KDF.ChatObjects
{
    /// <summary>
    /// Enthält Daten die den Client-User betreffen der in der jeweiligen Instanz des Client eingeloggt ist
    /// </summary>
    public class ClientUser
    {
        private Dictionary<string, Channel> _onlineChannels = new Dictionary<string, Channel>();
        /// <summary>
        /// Ruft ab in welchen Channels der Client sich befindet oder legt diese fest
        /// </summary>
        public Dictionary<string, Channel> OnlineChannels
        {
            get { return _onlineChannels; }
            set { _onlineChannels = value; }
        }

        private Channel _selectedChannel;
        /// <summary>
        /// Ruft ab welcher der aktive Channel ist oder legt ihn fest
        /// </summary>
        public Channel SelectedChannel
        {
            get { return _selectedChannel; }
            set { _selectedChannel = value; }
        }

        private string _username;
        /// <summary>
        /// Ruft den Usernamen des Users ab, welcher mit der aktuellen Instanz des Clients eingeloggt ist oder legt ihn fest
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;
        /// <summary>
        /// Ruft das passwort des Users ab, welcher mit der aktuellen Instanz des Clients eingeloggt ist oder legt es fest
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private List<string> _byNames = new List<string>();
        /// <summary>
        /// Ruft die im Chat eingestellten Spitznamen des Users ab, oder legt diese fest
        /// </summary>
        public List<string> ByNames
        {
            get { return _byNames; }
            set { _byNames = value; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ClientUser-Klasse.
        /// </summary>
        /// <param name="username">Der Username des Users der mit der aktuellen Instanz des Clients eingeloggt ist</param>
        /// <param name="password">Das Passwort des Users der mit der aktuellen Instanz des Clients eingeloggt ist</param>
        public ClientUser(string username, string password)
        {
            _username = username;
            _password = password;           
        }
    }
}
