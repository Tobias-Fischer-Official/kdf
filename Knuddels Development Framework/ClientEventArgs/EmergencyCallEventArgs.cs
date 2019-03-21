using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.ClientEventArgs
{
    public class EmergencyCallEventArgs : EventArgs
    {
        #region Returns
        private string _casenumber;
        /// <summary>
        /// Ruft die Fallnummer ab, gibt einen String zurück
        /// </summary>
        public string Casenumber
        {
            get { return _casenumber; }
        }

        private string _cdiaID;
        /// <summary>
        /// Ruft die cdiaID ab, gibt einen String zurück
        /// </summary>
        public string cdiaID
        {
            get { return _cdiaID; }
        }

        private string _reason;
        /// <summary>
        /// Ruft den Grund ab, gibt einen String zurück
        /// </summary>
        public string Reason
        {
            get { return _reason; }
        }

        private string _user;
        /// <summary>
        /// Ruft den User (Beschwerdeführer) ab
        /// </summary>
        public string User
        {
            get { return _user; }
        }

        private string _usercomment;
        /// <summary>
        /// Ruft das Userkommentar ab
        /// </summary>
        public string Usercomment
        {
            get { return _usercomment; }
        }
        #endregion

        #region Konstruktor
        public EmergencyCallEventArgs(string casenumber, string cdiaID, string reason, string user, string usercomment)
        {
            _casenumber = casenumber;
            _cdiaID = cdiaID;
            _reason = reason;
            _user = user;
            _usercomment = usercomment;
        }
        #endregion
    }
}
