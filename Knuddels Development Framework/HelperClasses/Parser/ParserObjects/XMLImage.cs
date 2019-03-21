using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.ParserObjects
{
    [Serializable]
    public class XMLImage
    {
        private byte[] _Data;
        public byte[] Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        private List<string> _Format = new List<string>();
        public List<string> Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        private string _Extension;
        public string Extension
        {
            get { return _Extension; }
            set { _Extension = value; }
        }

        public XMLImage()
        {

        }
    }
}
