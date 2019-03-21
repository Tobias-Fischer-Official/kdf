using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDF.Networks.CardServer;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das CardServerConnection-Ereignis bereit
    /// </summary>
    public class CardServerConnectionEventArgs : EventArgs
    {
        private CSClient _csClient;
        public CSClient Client
        {
            get { return _csClient; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der CardServerConnectionStateChangedEventArg-Klasse.
        /// </summary>
        /// <param name="csClient">Instanz des CardServer Clients</param>
        public CardServerConnectionEventArgs(CSClient csClient)
        {
            _csClient = csClient;
        }
    }
}