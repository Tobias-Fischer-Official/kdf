using System.Drawing;
using KDF.ChatObjects.Collections;

namespace KDF.ChatObjects
{
    /// <summary>
    /// Ein Chat-Channel
    /// </summary>
    public class Channel
    {
        private string _name;
        /// <summary>
        /// Der Name des Channels
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _nickJoined;
        /// <summary>
        /// Der Nick der diesen Channel betreten hat
        /// </summary>
        /// <remarks>Sollte immer der selbe Nick sein, wie der Client-Nick</remarks>
        public string NickJoined
        {
            get { return _nickJoined; }
            set { _nickJoined = value; }
        }
        private string _backgroundImage;
        /// <summary>
        /// Das Hintergrundbild des Channels
        /// </summary>
        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; }
        }
        private Color _foreColor;
        /// <summary>
        /// Die Vordergrund/Schriftfarbe des Channels
        /// </summary>
        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }
        private Color _backColor;
        /// <summary>
        /// Die Hintergrundfarbe des Channels
        /// </summary>
        /// <remarks>Wird im Client nur angezeigt, wenn das Hintergrundbild nicht auf 100%x100% gestrecht wird</remarks>
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }
        private Color _color1;
        /// <summary>
        /// Eine Spezialfarbe, die bei Eingabe von °RR° den nachfolgenden Text färbt
        /// </summary>
        public Color Color1
        {
            get { return _color1; }
            set { _color1 = value; }
        }
        private Color _color2;
        /// <summary>
        /// Eine Spezialfarbe, die bei Eingabe von °BB° den nachfolgenden Text färbt
        /// </summary>
        public Color Color2
        {
            get { return _color2; }
            set { _color2 = value; }
        }
        private Color _color3;
        /// <summary>
        /// Eine Spezialfarbe, deren Verwendung noch unbekannt ist
        /// </summary> 
        public Color Color3
        {
            get { return _color3; }
            set { _color3 = value; }
        }
        private int _fontSize;
        /// <summary>
        /// Die Schriftgröße des Channels in Pixeln
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }
        private int _linePitch;
        /// <summary>
        /// Der vertikale Zeilenabstand zwischen den Nachrichten in dem Channel
        /// </summary>
        /// <remarks>Hat nicht in allen Channels eine Wirkung/Funktion, zb. in FotoFlirt (durch die vorangestellten Thumbnails)</remarks>
        public int LinePitch
        {
            get { return _linePitch; }
            set { _linePitch = value; }
        }
        private int _spamTimeout;
        /// <summary>
        /// Die minimale Zeitspanne, in Millisekunden, die zwischen zwei öffentlichen Nachrichten liegen darf, ohne dass das Serverseitige Anti-Flood-System anläuft
        /// </summary>
        /// <remarks>Unterschreitet man diese Zeitspanne unterschreitet sagt James privat "Bitte nicht Spammen und Fluten"</remarks>
        public int SpamTimeout
        {
            get { return _spamTimeout; }
            set { _spamTimeout = value; }
        }

        private UserList _userList;
        /// <summary>
        /// Die Liste aller sich in dem Channel befindlichen Chat-User
        /// </summary>
        public UserList UserList
        {
            get { return _userList; }
            set { _userList = value; }
        }


        /// <summary>
        /// Erstellt eine neue Instanz eines Channels
        /// </summary>
        /// <param name="name">Der Channelname</param>
        /// <param name="nickJoined">Der Nick, der den Channel betreten hat</param>
        /// <param name="color1">Die erste Spezialfarbe (°BB°)</param>
        /// <param name="color2">Die zweite Spezialfarbe (°RR°)</param>
        /// <param name="color3">Die dritte Spezialfarbe (?)</param>
        /// <param name="fontSize">Die Schriftgröße</param>
        /// <param name="linePitch">Der vertikale Zeileabstand</param>
        /// <param name="spamTimeout">Die minimale Zeitspanne, in Millisekunden, die zwischen zwei öffentlichen Nachrichten liegen darf</param>
        /// <example><code>
        /// Channel channel = new Channel("Knuddels", "Holgi", Color.Red, Color.Blue, Color.Cyan, 15, 5, 3000);
        /// </code></example>        
        public Channel(string name, string nickJoined,Color foreColor, Color backColor, Color color1, Color color2, Color color3, int fontSize, int linePitch, int spamTimeout)
        {
            _name = name;
            _nickJoined = nickJoined;
            _foreColor = foreColor;
            _backColor = backColor;
            _color1 = color1;
            _color2 = color2;
            _color3 = color3;
            _fontSize = fontSize;
            _linePitch = linePitch;
            _spamTimeout = spamTimeout;
        }

        /// <summary>
        /// Erstellt eine neue Instanz eines Channels
        /// </summary>
        /// <param name="name">Der Channelname</param>      
        /// <example><code>
        /// Channel channel = new Channel("Knuddels");
        /// </code></example>        
        public Channel(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Gibt an ob ein User mit einbem bestimmten Nick sich in dem Channel aufhällt
        /// </summary>
        /// <param name="userName">Der Name des Users</param>
        /// <returns>Ob der User sich im Channel befindet</returns>
        public bool ContainsUser(string userName)
        {
            foreach (ChannelUser cu in _userList.ChannelUserList)
                if (cu.Name.ToLower() == userName.ToLower())
                    return true;
            return false;
        }
    }
}
