using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.Popup
{
    internal class PopupReader
    {
        #region private Vars
        private byte[] _charByte;
        private byte[] _buffer;
        private int _offset;
        #endregion

        #region Konstruktor
        public PopupReader(byte[] buffer)
        {
            this._buffer = buffer;
            this._charByte = new byte[256];
        }
        #endregion

        #region Skip / Back
        public void Skip(int pLength)
        {
            this._offset += pLength;
        }

        public void Back(int pLength)
        {
            this._offset -= pLength;
        }
        #endregion

        #region Read
        public byte Read()
        {
            return this._buffer[(_offset++)];
        }
        public byte Read(int pIndex)
        {
            return this._buffer[pIndex];
        }
        
        #region ReadString
        public string ReadString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = this._offset; i < this._buffer.Length; i++)
            {
                int c = Read();
                if (c == 0xF5)
                    break;
                buffer.Append((char)c);
            }
            return buffer.ToString();
        }
        public string ReadString(int i)
        {
            char pChar = (char)i;
            if (Read() != 0x7E)
            {
                Back(1);
                return (new StringBuilder()).Append("").Append(pChar).Append(_charByte[pChar & 0xFF]++).ToString();
            }
            return ReadString();
        }
        #endregion

        #region ReadBoolean/End
        public bool ReadBoolean()
        {
            return ((char)Read()).ToString().ToLower()[0] == 0x74;
        }

        public bool End()
        {
            if (Read(this._offset) == 0xE3)
            {
                Skip(1);
                return true;
            }
            return false;
        }
        #endregion

        #region ReadShort/ReadSize
        public int ReadShort()
        {
            return Read() << 8 | Read();
        }
        
        public int ReadSize()
        {
            return Read() - 0x41;
        }
        #endregion

        #region ReadColor
        public System.Drawing.Color ReadColor()
        {
            return System.Drawing.Color.FromArgb(Read(), Read(), Read());
        }
        #endregion

        #endregion
    }
}
