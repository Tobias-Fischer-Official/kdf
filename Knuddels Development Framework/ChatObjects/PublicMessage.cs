namespace KDF.ChatObjects
{
    /// <summary>
    /// Instanz einer Öffentlichen Nachricht
    /// </summary>    

    public class PublicMessage
    {
        private string _sender;
        /// <summary>
        /// Der Absender der öffentlichen Nachricht
        /// </summary>
        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private string _channel;
        /// <summary>
        /// Der Channel für den diese Nachricht bestimmt ist
        /// </summary>
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private string _message;
        /// <summary>
        /// Die Nachricht
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _leadingImage;
        /// <summary>
        /// Das vorrangestellte Bild
        /// </summary>
        public string LeadingImage
        {
            get { return _leadingImage; }
            set { _leadingImage = value; }
        }

        private bool _function;
        /// <summary>
        /// Bestimmt ob diese Nachricht das Ergebnis der Eingabe einer /-Funktion ist (zb. /me)
        /// </summary>
        public bool Function
        {
            get { return _function; }
            set { _function = value; }
        }

        /// <summary>
        /// Erstellt eine Instanz einer eingehenden öffentlichen Nachricht
        /// </summary>
        /// <param name="packets">Das string[], welches die Datenpakete vom Knuddelsserver beinhaltet</param>
        /// <param name="selectedChannelName">Gibt den Channel an, in welchem sich der Client gerade befindet und aktiv ist</param>
        /// <remarks>
        /// Im Konstruktor wird auch sofort festgelegt, ob es eine /-Funktion (wie zb. /me) ist, und ob vor dem Nick ein Bild erscheinen soll (zb. wie bei /kiss)
        /// </remarks>        
        public PublicMessage(string[] packets, string selectedChannelName)
        {
            _sender = packets[1];
            _function = _sender == ">" || _sender == ">>" ? true : false;
            _channel = packets[2] == "-" ? selectedChannelName : packets[2];
            _message = packets[3];
            _leadingImage = packets.Length > 4 ? packets[4] : null;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der PublicMessage-Klasse.
        /// </summary>
        /// <param name="sender">Der Absender-Nick</param>
        /// <param name="channel">Der Ziel-Channel</param>
        /// <param name="message">Die Nachricht</param>
        /// <remarks>
        /// Dieser Konstruktor sollte aufgerufen werden, wenn der Client sich in mehreren Channels befindet,
        /// da hier der Zielchannel mit angegeben werden muss
        /// </remarks>
        /// <example>
        /// <code>
        /// PublicMessage publicMessage = new PublicMessage("Holgi", "Knuddels", "Hallo Knuddels");
        /// </code>
        /// </example>
        public PublicMessage(string sender, string channel, string message)
        {
            _sender = sender;
            _channel = channel;
            _message = message;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der PublicMessage-Klasse.
        /// </summary>
        /// <param name="channel">Der Ziel-Channel</param>
        /// <param name="message">Die Nachricht</param>
        /// <remarks>
        /// Dieser Konstruktor sollte aufgerufen werden, wenn der Client sich in mehreren Channels befindet,
        /// da hier der Zielchannel mit angegeben werden muss
        /// </remarks>
        /// <example>
        /// <code>
        /// PublicMessage publicMessage = new PublicMessage("Knuddels", "Hallo Knuddels");
        /// </code>
        /// </example>
        public PublicMessage(string channel, string message)
        {
            _channel = channel;
            _message = message;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der PublicMessage-Klasse.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        /// <remarks>
        /// Dieser Konstruktor sollte aufgerufen werden, wenn der Client sich in einem Channel befindet
        /// </remarks>
        /// <example>
        /// <code>
        /// PublicMessage publicMessage = new PublicMessage("Hallo Knuddels");
        /// </code>
        /// </example>
        public PublicMessage(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der PublicMessage-Klasse.
        /// </summary>
        /// <remarks>
        /// Parameterloser Konstruktor
        /// </remarks>
        /// <example>
        /// <code>
        /// PublicMessage publicMessage = new PublicMessage();
        /// </code>
        /// </example>        
        public PublicMessage() { }
    }
}
