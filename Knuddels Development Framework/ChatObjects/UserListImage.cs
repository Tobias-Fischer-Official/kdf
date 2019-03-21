namespace KDF.ChatObjects
{
    /// <summary>
    /// Stell ein Objekt zur Verfügung, welches die Daten eines Bildes aus der Userliste enthält
    /// </summary>
    public class UserListImage
    {
        private string _user;
        /// <summary>
        /// Ruft den Namen des Users ab, zu welchem das Bild gehört
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        private string _channel;
        /// <summary>
        /// Ruft den Channel ab, in welchem sich der betreffende User befindet
        /// </summary>
        /// <remarks>Kann nur ein Channel sein, in welchem der mit dem Client eingeloggte User ebenfalls anwesend ist</remarks>
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        private string _image;
        /// <summary>
        /// Gibt das Bild an, welches hinzugefügt/entfernt werden soll
        /// </summary>
        /// <remarks>Hier wird nur die URL zu dem Bild übergeben</remarks>
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">Gibt den Usernamen an</param>
        /// <param name="channel">Gibt den Channelnamen an</param>
        /// <param name="image">Gibt das Bild (als URL) an, welches entfernt oder hinzugefügt werden soll</param>
        public UserListImage(string user, string channel, string image)
        {
            _user = user;
            _channel = channel;
            _image = image;
        }
    }
}
