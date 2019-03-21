using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.ClientEventArgs
{
    public class SecurityWatcherEventArgs : EventArgs
    {
        #region Returns
        private SecurityAlertReason _reason;
        /// <summary>
        /// Ruft den Grund ab, warum der Alarm ausgelöst wurde
        /// </summary>
        public SecurityAlertReason Reason
        {
            get { return _reason; }
        }
        private string _adminNick;
        /// <summary>
        /// Ruft den AdminNick der den Alarm ausgelöst hat
        /// </summary>
        public string AdminNick
        {
            get { return _adminNick; }
        }
        private int _code;
        /// <summary>
        /// Ruft den Code zum bestätigen einer Botkontrolle oder eines Invites ab
        /// </summary>
        public int Code
        {
            get { return _code; }
        }
        private string _message;
        /// <summary>
        /// Ruft die Nachricht ab die erhalten wurde
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        private string _toSend;
        /// <summary>
        /// Ruft die Text ab der gesendet werden soll
        /// </summary>
        public string ToSend
        {
            get { return _toSend; }
        }

        private string _subject;
        /// <summary>
        /// Ruft den Betreff ab
        /// </summary>
        public string Subject
        {
            get { return _subject; }
        }

        private string _signature;
        /// <summary>
        /// Ruft den User (Beschwerdeführer) ab
        /// </summary>
        public string Signature
        {
            get { return _signature; }
        }
        private string _channel;
        /// <summary>
        /// Ruft den Channel ab aus dem die Aktion erfolgte
        /// </summary>
        public string Channel
        {
            get { return _channel; }
        }
        #endregion

        #region Konstruktoren
        /// <summary>
        /// Initialisiert eine neue Instanz der SecurityWatcherEventArgs-Klasse.
        /// </summary>
        /// <param name="reason">Gibt den Grund für den ausgelösten Alarm an</param>
        /// <param name="toSend">Gibt an was für ein Text zum Senden übergeben werden soll</param>
        public SecurityWatcherEventArgs(string toSend)
        {
            _reason = SecurityAlertReason.SendUserInputForMe;
            _toSend = toSend;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der SecurityWatcherEventArgs-Klasse.
        /// </summary>
        /// <param name="reason">Gibt den Grund für den ausgelösten Alarm an</param>
        /// <param name="adminNick">Gibt den Adminnick an der den Alarm ausgelöst hat an</param>
        public SecurityWatcherEventArgs(SecurityAlertReason reason, string adminNick)
        {
            _reason = reason;
            _adminNick = adminNick;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der SecurityWatcherEventArgs-Klasse.
        /// </summary>
        /// <param name="reason">Gibt den Grund für den ausgelösten Alarm an</param>
        /// <param name="adminNick">Gibt den Adminnick an der den Alarm ausgelöst hat an</param>
        /// <param name="code">Gibt den Code an der bei einer Botkontrolle/einem Invite erhalten wird</param>
        public SecurityWatcherEventArgs(SecurityAlertReason reason, string adminNick, string message, int code, string channel)
        {
            _reason = reason;
            _adminNick = adminNick;
            _code = code;
            _message = message;
            _channel = channel;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der SecurityWatcherEventArgs-Klasse.
        /// </summary>
        /// <param name="sender">Gibt den Sender der Mail an</param>
        /// <param name="subject">Gibt den Betreff der Mail an</param>
        /// <param name="message">Gibt die Nachricht der Mail an</param>
        /// <param name="signature">Gibt die Signatur vom Sender an</param>
        public SecurityWatcherEventArgs(string sender, string subject, string message, string signature)
        {
            _reason = SecurityAlertReason.Mail;
            _adminNick = sender;
            _subject = subject;
            _message = message;
            _signature = signature;
        }
        #endregion

        #region Enum
        /// <summary>
        /// Gibt den Grund an, warum der Alarm ausgelöst wurde
        /// </summary>
        public enum SecurityAlertReason
        {
            /// <summary>
            /// Ein Admin hat den Channel betreten
            /// </summary>
            Joined,
            /// <summary>
            /// Ein Admin hat mich öffentlich angeschrieben
            /// </summary>
            Public,
            /// <summary>
            /// Ein Admin hat mich privat angeschrieben
            /// </summary>
            Private,
            /// <summary>
            /// Ein Admin hat mir eine /m geschrieben
            /// </summary>
            Mail,
            /// <summary>
            /// Eine Botkontrolle erhalten
            /// </summary>
            BotCheck,
            /// <summary>
            /// Ein Invite wurde erhalten
            /// </summary>
            Invite,
            /// <summary>
            /// UserInput soll für den SecurityWatcher gesendet werden (/m's öffnen)
            /// </summary>
            SendUserInputForMe
        }
        #endregion
    }
}
