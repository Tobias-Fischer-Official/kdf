using System.Collections.Generic;

namespace KDF.ChatObjects
{
    /// <summary>
    /// Ein Channel ind er Globalen Channelliste (wie sie beim Login angezeigt wird)
    /// </summary>
    public class ChannellnGlobalList
    {
        private string _name;
        /// <summary>
        /// Ruft den Namen des Channels ab
        /// </summary>
        public string Name { get { return _name; } }

        private int _usersOnline;
        /// <summary>
        /// Ruft ab wieviele User sich in dem Channel befinden
        /// </summary>
        public int UsersOnline { get { return _usersOnline; } }

        private bool _restricted;
        /// <summary>
        /// Ruft ab ob der Channel Zugangsvorraussetzungen hat
        /// </summary>
        public bool Restricted { get { return _restricted; } }

        private bool _full;
        /// <summary>
        /// Ruft ab ob der Channel voll ist
        /// </summary>
        public bool Full { get { return _full; } }

        private List<string> _images;
        /// <summary>
        /// Ruft die Bilder die hinter dem Channelnamen stehen (full, event, etc.) ab
        /// </summary>
        public List<string> Images { get { return _images; } }

        private string _parentChannel;
        /// <summary>
        /// Ruft den übergeordneten Channel (zb. Flirt bei Flirt 10) ab
        /// </summary>
        public string ParentChannel { get { return _parentChannel; } }

        private bool _parent;
        /// <summary>
        /// Ruft ab ob der Channel der erste Channel ist oder ein Subchannel eines Channels
        /// </summary>
        public bool Parent { get { return _parent; } }

        /// <summary>
        /// Erstellt eine neue Instanz eines Channels der globalen Channelliste
        /// </summary>
        /// <param name="name">Gibt den Namen des Channels an</param>
        /// <param name="parentChannel">Gibt den übergeordneten Channel an</param>
        /// <param name="usersOnline">Gibt an wieviele User in dem Channel online sind</param>
        /// <param name="full">Gibt an ob der Channel voll ist</param>
        /// <param name="images">Gibt bie Bilder die dem Channel hintangestellt sind an</param>
        /// <param name="restricted">Gibt an ob der Channel Zugangsvorraussetzungen hat</param>
        /// <param name="parent">Gibt an ob der Channel ein Kind- oder Vater-Channel ist</param>
        /// <remarks>Hinweis: die Anzahl der User ist nur die zum Zeitpunkt des Einloggens und kann sich verändern, wird aber im Client nicht automatisch aktualisiert.</remarks>
        public ChannellnGlobalList(string name, string parentChannel, int usersOnline, bool full, List<string> images, bool restricted, bool parent)
        {
            _name = name;
            _parentChannel = parentChannel;
            _usersOnline = usersOnline;
            _full = full;
            _images = images;
            _restricted = restricted;
            _parent = parent;
        }
    }
}
