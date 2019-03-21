using KDF.ChatObjects;
using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnUserJoinedChannel-Ereignis bereit
    /// </summary>
    /// <seealso cref="ChannelUser"/>
    public class UserJoinedChannelEventArgs : EventArgs
    {
        private ChannelUser _channelUser;
        /// <summary>
        ///  Ruft den User ab oder legt diesen fest.
        /// </summary>
        public ChannelUser ChannelUser
        {
            get { return _channelUser; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der UserJoinedChannelEventArgs-Klasse.
        /// </summary>
        /// <param name="channelUser">Die empfangenen Daten</param>
        public UserJoinedChannelEventArgs(ChannelUser channelUser)
        {
            _channelUser = channelUser;
        }
    }
}
