using System;
using System.Collections.Generic;
using KDF.ChatObjects;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das On GlobalChannelListReceived-Ereignis bereit
    /// </summary>
    public class GlobalChannelListReceivedEventArgs : EventArgs
    {
        private List<ChannellnGlobalList> _globalChannelList;
        /// <summary>
        /// Ruf die globale Channelliste ab die Empfangen wurde
        /// </summary>
        public List<ChannellnGlobalList> GlobalChannelList
        {
            get { return _globalChannelList; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der GlobalChannelListReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="globalChannelList">Die globale Channelliste des Chats</param>
        public GlobalChannelListReceivedEventArgs(List<ChannellnGlobalList> globalChannelList)
        {
            _globalChannelList = globalChannelList;
        }
    }
}
