using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Graphics.ChatHistory
{
    public class LinkClickedEventArgs : EventArgs
    {
        private string _Command = null;
        public string Command
        {
            get { return _Command; }
        }
        public LinkClickedEventArgs(string Command)
        {
            this._Command = Command;
        }
    }
}
