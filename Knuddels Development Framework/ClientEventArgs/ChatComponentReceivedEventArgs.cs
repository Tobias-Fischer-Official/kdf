using System;
using KDF.Networks.Protocol.Module;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnChatComponentCommandReceived-Ereignis bereit
    /// </summary>
    public class ChatComponentReceivedEventArgs : EventArgs
    {
        private KnModule _module;
        /// <summary>
        /// Ruft das empfangene :-Token ab
        /// </summary>
        public KnModule Module
        {
            get { return _module; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ChatComponentCommandReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="data">Gibt das empfangene :-Token an</param>
        public ChatComponentReceivedEventArgs(KnModule module)
        {
            _module = module;
        }
    }
}
