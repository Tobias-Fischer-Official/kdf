using KDF.ChatObjects;
using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnPrivateMessageReceived-Ereignis bereit
    /// </summary>
    /// <seealso cref="PrivateMessage"/>
    public class PrivateMessageReceivedEventArgs : EventArgs
    {
        private PrivateMessage _privateMessage;
        /// <summary>
        /// Ruft die empfangene private Nachricht ab
        /// </summary>
        public PrivateMessage PrivateMessage
        {
            get { return _privateMessage; }
        }
        /// <summary>
        /// Initialisiert eine neue Instanz der KeyPressEventArgs-Klasse.
        /// </summary>
        /// <param name="privateMessage">Die empfangene private NAchricht</param>
        public PrivateMessageReceivedEventArgs(PrivateMessage privateMessage)
        {
            _privateMessage = privateMessage;
        }
    }
}
