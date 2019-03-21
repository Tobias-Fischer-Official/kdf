using KDF.ChatObjects;
using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnPublicMessageReceived-Ereignis bereit
    /// </summary>
    /// <seealso cref="PublicMessage"/>
    public class PublicMessageReceivedEventArgs : EventArgs
    {
        private PublicMessage _publicMessage;
        /// <summary>
        /// Ruft die Öffentliche Nachricht ab oder legt diese fest.
        /// </summary>
        public PublicMessage PublicMessage
        {
            get { return _publicMessage; }            
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der PublicMessageReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="publicMessage">Die <c>PublicMessage</c> die empfangen wurde</param>
        /// <seealso cref="PublicMessage"/>
        public PublicMessageReceivedEventArgs(PublicMessage publicMessage)
        {
            _publicMessage = publicMessage;
        }
    }
}
