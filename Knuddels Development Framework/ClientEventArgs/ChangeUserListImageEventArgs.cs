using System;
using KDF.ChatObjects;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnChangeUserListImage-Ereignis bereit
    /// </summary>
    /// <remarks>
    /// <para>Dieses Ereignis tritt dann ein, wenn der Server Das Token mit dem Token-Identifizierer 'm' oder 'z' an den Client sendet.</para>
    /// <para>Das m und das z Token sind beide gleich aufgebaut: m|z\0Channek\0UserName\0ImageURL</para>
    /// <para></para>
    /// </remarks>
    public class ChangeUserListImageEventArgs : EventArgs
    {
        private UserListImage _userListImage;
        /// <summary>
        /// Ruft das Bild-Objekt ab, welches die Daten beinhaltet
        /// </summary>
        public UserListImage UserListImage
        {
            get { return _userListImage; }
            set { _userListImage = value; }
        }
        private bool _addOrRemove;
        /// <summary>
        /// Ruft ab, ob das Bild entfernt, oder hinzugefügt werden soll
        /// </summary>
        public bool AddOrRemove
        {
            get { return _addOrRemove; }
            set { _addOrRemove = value; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der ChangeUserListImageEventArgs-Klasse.
        /// </summary>
        /// <param name="userListImage">Gibt die Daten an, welche zum Bild gehören, welches entfernt oder hinzugefügt werden soll</param>
        /// <param name="addOrRemove">Gibt an ob das Bild entfernt oder hinzugefügt werden soll (true = hinzufügen, false = entfernen)</param>
        public ChangeUserListImageEventArgs(UserListImage userListImage, bool addOrRemove)
        {
            _userListImage = userListImage;
            _addOrRemove = addOrRemove;
        }
    }
}
