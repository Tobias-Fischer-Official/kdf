using System;
using System.Collections.Generic;

namespace KDF.ChatObjects
{
    /// <summary>
    /// Instanz einer privaten Nachricht
    /// </summary>
    public class PrivateMessage
    {
        private List<string> _receivers = new List<string>();
        /// <summary>
        /// Der/Die Empfänger der privaten Nachricht
        /// </summary>
        public List<string> Receivers
        {
            get { return _receivers; }
            set { _receivers = value; }
        }

        private string _sender;
        /// <summary>
        /// Der Absender der privaten Nachricht
        /// </summary>
        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private string _fromChannel;
        /// <summary>
        /// Der Channel, aus welchem die private Nachricht abgeschickt wurde
        /// </summary>
        public string FromChannel
        {
            get { return _fromChannel; }
            set { _fromChannel = value; }
        }

        private string _toChannel;
        /// <summary>
        /// Gibt an an welchen Channel die private Nachricht ging
        /// </summary>
        public string ToChannel
        {
            get { return _toChannel; }
        }

        private string _message;
        /// <summary>
        /// Die private Nachricht
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Erstellt eine Instanz einer eingehenden privaten Nachricht
        /// </summary>
        /// <param name="packets">Das string[], welches die Datenpakete vom Knuddelsserver beinhaltet</param>
        /// <param name="selectedChannelName">Gibt den Channel an, in welchem sich der Client gerade befindet und aktiv ist</param>
        /// <remarks></remarks>
        public PrivateMessage(string[] packets, string selectedChannelName)
        {
            _sender = packets[1];
            _receivers.AddRange(packets[2].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            _toChannel = packets[3] == "-" ? selectedChannelName : packets[3];
            _message = packets[4];
            _fromChannel = packets[5] == " " ? selectedChannelName : packets[5];
        }

        /// <summary>
        /// Erstellt eine Instanz einer ausgehenden privaten Nachricht
        /// </summary>
        /// <param name="message">Die private Nachricht</param>
        /// <param name="receivers">Die Empfänger der privaten Nachricht</param>
        /// <remarks></remarks>
        public PrivateMessage(string message, string[] receivers)
        {
            _message = message;
            _receivers.AddRange(receivers);
        }

        /// <summary>
        /// Erstellt eine Instanz einer privaten Nachricht
        /// </summary>
        public PrivateMessage() { }
    }
}
