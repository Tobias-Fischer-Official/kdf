using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das On  ChannelBackGroundChanged-Ereignis bereit
    /// </summary>
    public class ChannelBackGroundChangedEventArgs : EventArgs
    {
        private string _backgroundImage;
        /// <summary>
        /// Ruft das Hintergrundbild ab
        /// </summary>
        public string BackgroundImage
        {
            get { return _backgroundImage; }           
        }

        private string _channelName;
        /// <summary>
        /// Ruft den Namen des Channels ab, für welchen das Hintergrundbild geändert werden soll
        /// </summary>
        public string ChannelName
        {
            get { return _channelName; }
        }
        /// <summary>
        ///  Initialisiert eine neue Instanz der ChannelBackGroundChangedEventArgs-Klasse.
        /// </summary>
        /// <param name="backgroundImage">Gibt den link zum neuen Hintergrundbild an</param>
        /// <param name="channelName">Gibt den Namen des Channels an, dessen Hintergrundbild geändert werden soll</param>
        public ChannelBackGroundChangedEventArgs(string backgroundImage, string channelName)
        {
            _backgroundImage = backgroundImage;
            _channelName = channelName;
        }
    }
}
