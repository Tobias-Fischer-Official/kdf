using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDF.Networks.GameServer;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das GameServerConnection-Ereignis bereit
    /// </summary>
    public class GameServerConnectionEventArgs : EventArgs
    {
        private GSClient _csClient;
        public GSClient Client
        {
            get { return _csClient; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der GameServerConnectionStateChangedEventArg-Klasse.
        /// </summary>
        /// <param name="csClient">Instanz des GameServer Clients</param>
        public GameServerConnectionEventArgs(GSClient csClient)
        {
            _csClient = csClient;
        }
    }
}
