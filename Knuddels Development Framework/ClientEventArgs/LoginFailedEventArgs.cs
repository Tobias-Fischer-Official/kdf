using System;

namespace KDF.ClientEventArgs
{
    /// <summary>
    /// Stellt Daten für das OnLoginFailed-Ereignis bereit
    /// </summary>
    public class LoginFailedEventArgs : EventArgs
    {
        private LoginFailReason _reason;
        /// <summary>
        /// Ruft ab, was der Grund für den missglückten Loginversuch war
        /// </summary>
        public LoginFailReason Reason
        {
            get { return _reason; }
        }

        private string _textReason;
        /// <summary>
        /// Ruft bei einer Nicksperre den vom Admin angegebenen Grund für diese Sperre ab
        /// </summary>
        public string TextReason
        {
            get { return _textReason; }
        }

        private string _adminNick;
        /// <summary>
        /// Ruft bei einer Nicksperre den sperrenden Adminnick ab
        /// </summary>
        public string AdminNick
        {
            get { return _adminNick; }
        }

        private string _lockLast;
        /// <summary>
        /// Ruf bei einer Nicksperre den Zeitraum ab, für den der Nick gesperrt wurde
        /// </summary>
        public string LockLast
        {
            get { return _lockLast; }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der LoginFailedEventArgs-Klasse.
        /// </summary>
        /// <param name="reason">Gibt an, was der Grund für den missglückten Loginversuch war</param>
        public LoginFailedEventArgs(LoginFailReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der LoginFailedEventArgs-Klasse.
        /// </summary>
        /// <param name="reason">Gibt an, was der Grund für den missglückten Loginversuch war</param>
        /// <param name="textReason">Gibt an welche Begründung im Sperrtext steht</param>
        /// <param name="adminNick">Gibt an welcher Admin den Nick gesperrt hat</param>
        /// <param name="lockLast">Gibt an, für welchen Zeitraum der Nick gesperrt wurde</param>
        public LoginFailedEventArgs(LoginFailReason reason, string textReason, string adminNick, string lockLast)
        {
            _reason = reason;
            _textReason = textReason;
            _adminNick = adminNick;
            _lockLast = lockLast;
        }
    }

    /// <summary>
    /// Gibt den Grund an, weshalb ein Login fehlschlug
    /// </summary>
    public enum LoginFailReason
    {
        /// <summary>
        /// Der User wurde gesperrt
        /// </summary>
        UserLocked,
        /// <summary>
        /// Der Knuddels-Server ist nicht verfügbar
        /// </summary>
        ServerNotAvailable,
        /// <summary>
        /// Der Nickname oder das Passwort sind falsch
        /// </summary>
        WrongUsernameOrPassword,
        /// <summary>
        /// Der angegebene Channel existiert nicht
        /// </summary>
        ChannelDoesntExist,
        /// <summary>
        /// Der Grund für den fehlgeschlagenen Login konnte nicht von der Library ermittelt werden
        /// </summary>        
        Unknown,
        /// <summary>
        /// Der Channel hat Zugangsvorraussetzungen, welche der Nick nicht erfüllt
        /// </summary>
       UserDoesntMeetChannelRestrictions,
        /// <summary>
        /// Das verwendete Applet ist zu alt
        /// </summary>
        AppletTooOld,
        /// <summary>
        /// Der Channel ist voll
        /// </summary>
        ChannelIsFull,
        /// <summary>
        /// Die Ip wurde gesperrt
        /// </summary>
        IPLock
    }
}
