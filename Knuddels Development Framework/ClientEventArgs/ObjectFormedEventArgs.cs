using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.ClientEventArgs
{
    public class ObjectFormedEventArgs : EventArgs
    {
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private object _receivedObject;
        public object ReceivedObject
        {
            get { return _receivedObject; }
            set { _receivedObject = value; }
        }

        public ObjectFormedEventArgs(string type, object receivedObvject)
        {
            _receivedObject = receivedObvject;
            _type = type;
        }
    }
}
