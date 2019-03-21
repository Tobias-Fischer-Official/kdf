using System;
using KDF.ChatObjects;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das On  ChannelJoinedReceived-Ereignis bereit
    /// </summary>
    public class ChannelJoinedEventArgs : EventArgs
    {
        private Channel _joinedChannel;
        /// <summary>
        /// Ruft ab welcher Channel betreten wurde
        /// </summary>
        public Channel JoinedChannel
        {
            get { return _joinedChannel; }            
        }
        /// <summary>
        /// Initialisiert eine neue Instanz der ChannelJoinedEventArgs-Klasse.
        /// </summary>
        /// <param name="joinedChannel">Der Channel, welcher betreten wurde</param>
        public ChannelJoinedEventArgs(Channel joinedChannel)
        {
            _joinedChannel = joinedChannel;
        }
    }
}
