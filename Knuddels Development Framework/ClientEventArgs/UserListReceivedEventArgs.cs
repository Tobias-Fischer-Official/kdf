using System;
using KDF.ChatObjects.Collections;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das On  UserListReceived-Ereignis bereit
    /// </summary>
    public class UserListReceivedEventArgs : EventArgs
    {
        private UserList _userList;
        /// <summary>
        /// Ruft die UserList ab, die empfangen wurde
        /// </summary>
        public UserList UserList
        {
            get { return _userList; }
        }
        /// <summary>
        /// Initialisiert eine neue Instanz der UserListReceivedEventArgs-Klasse.
        /// </summary>
        /// <param name="userList">Die empfangene Userliste</param>
        public UserListReceivedEventArgs(UserList userList)
        {
            _userList = userList;
        }
    }
}