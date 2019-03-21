using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Graphics.UserList
{
    public class ItemSelectedChangedEventArgs : EventArgs
    {
        private bool _Selected;
        public bool Selected { get { return _Selected; } }

        private UserListItemControl _Item;
        public UserListItemControl Item { get { return _Item; } }

        public ItemSelectedChangedEventArgs(bool Selected, UserListItemControl Item)
        {
            _Selected = Selected;
            _Item = Item;
        }
    }
}
