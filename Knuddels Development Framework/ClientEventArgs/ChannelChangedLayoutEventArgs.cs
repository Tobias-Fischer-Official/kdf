using System;
using KDF.ChatObjects;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnChannelChanged-Ereignis bereit
    /// </summary>
    public class ChannelChangedLayoutEventArgs : EventArgs
    {
        private Channel _channelWithNewLayout;

        /// <summary>
        /// Ruft den Namen des Channels ab, welcher betreten wurde
        /// </summary>
        public Channel channelWithNewLayout
        {
            get { return _channelWithNewLayout; }
            set { _channelWithNewLayout = value; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ChannelChangedEventArgs-Klasse.
        /// </summary>
        /// <param name="channelWithNewLayout">Der Channel, dessen Layout gewechselt wurde</param>
        public ChannelChangedLayoutEventArgs(Channel channelWithNewLayout)
        {
            _channelWithNewLayout = channelWithNewLayout;
        }
    }
}
