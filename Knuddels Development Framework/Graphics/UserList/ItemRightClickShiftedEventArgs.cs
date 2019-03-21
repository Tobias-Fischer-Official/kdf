using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Graphics.UserList
{
    public class ItemRightClickShiftedEventArgs : EventArgs
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public ItemRightClickShiftedEventArgs(string message)
        {
            _message = message;
        }
    }
}
