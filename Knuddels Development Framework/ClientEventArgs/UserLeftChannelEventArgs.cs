using System;
using KDF.ChatObjects;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnUserLeftChannel-Ereignis bereit
    /// </summary>
    public class UserLeftChannelEventArgs : EventArgs
    {
        private UserLeftChannel _userLeftChannel;
        /// <summary>
        /// Ruft den User ab, welcher den Channel verlassen hat
        /// </summary>
        public UserLeftChannel UserLeftChannel
        {
            get { return _userLeftChannel; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der UserLeftChannelEventArgs-Klasse.
        /// </summary>
        /// <param name="userLeftChannel">Gibt den Channel an, welcher verlassen wurde</param>
        public UserLeftChannelEventArgs(UserLeftChannel userLeftChannel)
        {
            _userLeftChannel = userLeftChannel;
        }
    }
}
