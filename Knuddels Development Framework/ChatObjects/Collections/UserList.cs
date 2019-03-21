using System.Collections.Generic;
using System;
using System.Collections;

namespace KDF.ChatObjects.Collections
{
    /// <summary>
    /// Eine Liste von Usern, welche sich in einem bestimmten Channel befinden 
    /// </summary>
    [Serializable]
    public class UserList
    {
        private List<ChannelUser> _channelUserList = new List<ChannelUser>();
        public List<ChannelUser> ChannelUserList
        {
            get { return _channelUserList; }
            set { _channelUserList = value; }
        }

        private string _ownerChannel;
        /// <summary>
        /// Ruft den Channel, zu welchem diese Userlist gehört, ab
        /// </summary>
        public string OwnerChannel
        {
            get { return _ownerChannel; }
        }

        /// <summary>
        /// Erstellt eine neue Userliste für einen Channel
        /// </summary>
        /// <param name="ownerChannel">Der Channel, zu welchem die Userliste gehört</param>
        public UserList(string ownerChannel)
        {
            _ownerChannel = ownerChannel;
        }

        /// <summary>
        /// Entfernt einen <c>ChannelUser</c> von der <c>UserList</c>
        /// </summary>
        /// <param name="username">Gibt den Namen an, welcher von der Liste entfernt werden soll</param>
        public void RemoveByName(string username)
        {
            ChannelUser toRemove = null;
            foreach (ChannelUser channelUser in this._channelUserList)
                if (channelUser.Name == username)
                    toRemove = channelUser;
            if (toRemove != null)
                this._channelUserList.Remove(toRemove);
        }

        /// <summary>
        /// Sucht einen User anhand des Usernamens aus der Liste
        /// </summary>
        /// <param name="username">Der Name des USers der gesucht werden soll</param>
        /// <returns>Den User mit dem gesuchten Namen, wenn kein USer mit diesem Namen in der Liste steht, gibt diese Methode 'null' zurück</returns>
        public ChannelUser GetByName(string username)
        {
            foreach (ChannelUser channelUser in this._channelUserList)
                if (channelUser.Name == username)
                    return channelUser;
            return null;
        }

    }
}
