using System.Drawing;
using System.Collections.Generic;

namespace KDF.ChatObjects
{
    /// <summary>
    /// Ein in einem Channel befindlicher User
    /// </summary>
    public class ChannelUser
    {
        private string _name;
        /// <summary>
        /// Der Username
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int? _age;
        /// <summary>
        /// Das Alter des Users 
        /// </summary>
        /// <remarks>Wenn kein Alter gesetzt ist, ist der Wert null</remarks>
        public int? Age
        {
            get { return _age; }
            set { _age = value; }
        }

        private char _gender;
        /// <summary>
        /// Das Geschlecht des Users
        /// </summary>
        /// <remarks>
        /// w = weiblich
        /// m = männlich
        /// n = nicht angegeben
        /// </remarks>
        public char Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private Color _foreColor;
        /// <summary>
        /// Die Vordergrundfarbe des Users in der Nickliste
        /// </summary>
        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        private char _fontFormat;
        /// <summary>
        /// Die Formatierung des Nicks in der Nickliste
        /// </summary>
        /// <remarks>Fett oder Kursiv</remarks>
        public char FontFormat
        {
            get { return _fontFormat; }
            set { _fontFormat = value; }
        }

        private List<string> _userListImages;
        /// <summary>
        /// Die Bilder die hinter dem Nick in der Userliste erscheinen sollen
        /// </summary>
        public List<string> UserListImages
        {
            get { return _userListImages; }
            set { _userListImages = value; }
        }

        private string _channelLeaved;
        /// <summary>
        /// Der Channel den der User verlassen hat
        /// </summary>
        /// <remarks>Nur zu nutzen bei einem Join</remarks>
        public string ChannelLeaved
        {
            get { return _channelLeaved; }
            set { _channelLeaved = value; }
        }

        private string _channelJoined;
        /// <summary>
        /// Den Channel den der User betreten hat
        /// </summary>
        /// <remarks>Nur zu verwenden bei einem Join</remarks>
        public string ChannelJoined
        {
            get { return _channelJoined; }
            set { _channelJoined = value; }
        }

        /// <summary>
        /// Erstellt eine neue Instanz eines sich in einem Channel befindlichen Users
        /// </summary>
        /// <remarks>Nur zu verwenden, bei Usern die mit dem u-packet übergeben wurden</remarks>
        /// <param name="name">Der Username</param>
        /// <param name="age">Das Alter des Users</param>
        /// <param name="gender">Das Geschlecht des Users</param>
        /// <param name="foreColor">Die Schriftfarbe des Usernamens in der Userliste</param>
        /// <param name="fontFormat">Die Schriftformatierung des Usernamens in der Userliste</param>
        /// <param name="userListImages">Die Bilder die dem Usernamen in der Userliste hintangestellt sind</param>
        public ChannelUser(string name, int age, char gender, Color foreColor, char fontFormat, List<string> userListImages)
        {
            _name = name;
            _age = age;
            _gender = gender;
            _foreColor = foreColor;
            _fontFormat = fontFormat;
            _userListImages = userListImages;
        }

        /// <summary>
        /// Erstellt eine neue Instanz eines sich in einem Channel befindlichen Users
        /// </summary>
        /// <remarks>Nur zu verwenden, bei Usern die mit dem l-packet übergeben wurden</remarks>
        /// <param name="name">Der Username</param>
        /// <param name="age">Das Alter des Users</param>
        /// <param name="gender">Das Geschlecht des Users</param>
        /// <param name="foreColor">Die Schriftfarbe des Usernamens in der Userliste</param>
        /// <param name="fontFormat">Die Schriftformatierung des Usernamens in der Userliste</param>
        /// <param name="userListImages">Die Bilder die dem Usernamen in der Userliste hintangestellt sind</param>
        /// <param name="channelLeaved">Der Channel aus dem der User in den Aktuellen Channel gewechselt hat</param>
        /// <param name="channelJoined">Der Channel in den der User gerade gewechselt hat</param>
        public ChannelUser(string name, int age, char gender, Color foreColor, char fontFormat, List<string> userListImages, string channelLeaved, string channelJoined)
        {
            _name = name;
            _age = age;
            _gender = gender;
            _foreColor = foreColor;
            _fontFormat = fontFormat;
            _userListImages = userListImages;
            _channelLeaved = channelLeaved;
            _channelJoined = channelJoined;
        }
    }
}
