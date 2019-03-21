namespace KDF.ChatObjects
{
    /// <summary>
    /// Ein User, welcher einen Channel verlassen hat
    /// </summary>
    public class UserLeftChannel
    {
        private string _name;
        /// <summary>
        /// Ruft den Namen des Users ab, welcher den Channel verlassen hat
        /// </summary>
        public string Name
        {
            get { return _name; }            
        }
        private string _channelLeft;
        /// <summary>
        /// Ruft ab, welcher Channel verlassen wurde
        /// </summary>
        public string ChannelLeft
        {
            get { return _channelLeft; }
            set { _channelLeft = value; }

        }
        private string _channelJoined;
        /// <summary>
        /// Ruft ab, welcher Channel betreten wurde
        /// </summary>
        public string ChannelJoined
        {
            get { return _channelJoined; }
        }
        private string _unknown1;
        /// <summary>
        /// Ruft den 1. unbekannten Parameter ab
        /// </summary>
        public string Unknown1
        {
            get { return _unknown1; }
        }
        private string _unknown2;
        /// <summary>
        /// Ruft den 2. unbekannten Parameter ab
        /// </summary>
        public string Unknown2
        {
            get { return _unknown2; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der UserLeftChannel-Klasse.
        /// </summary>
        /// <param name="name">Gibt den Namen des Users an</param>
        /// <param name="channelLeft">Gibt an, welcher Channel verlassen wurde</param>
        /// <param name="channelJoined">Gibt an, welcher Channel betreten wurde</param>
        /// <param name="unknown1">Ein bisher unbekannter Parameter</param>
        /// <param name="unknown2">Ein bisher unbekannter Parameter</param>
        /// <remarks>Diese Klasse behandelt einen User, welche einen Channel verlassen hat</remarks>
        public UserLeftChannel(string name, string channelLeft, string channelJoined, string unknown1, string unknown2)
        {
            _name = name;
            _channelLeft = channelLeft;
            _channelJoined = channelJoined;
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der UserLeftChannel-Klasse.
        /// </summary>
        /// <param name="name">Gibt den Namen des Users an</param>
        /// <param name="channelLeft">Gibt an, welcher Channel verlassen wurde</param>
        /// <param name="channelJoined">Gibt an, welcher Channel betreten wurde</param>
        /// <param name="unknown1">Ein bisher unbekannter Parameter</param>
        /// <remarks>Diese Klasse behandelt einen User, welche einen Channel verlassen hat</remarks>
        public UserLeftChannel(string name, string channelLeft, string channelJoined, string unknown1)
        {
            _name = name;
            _channelLeft = channelLeft;
            _channelJoined = channelJoined;
            _unknown1 = unknown1;
        }
    }
}
