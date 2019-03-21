using System;
using System.Text;
using System.IO;

namespace KDF.Networks.Protocol.Module
{
    internal class PacketWriter : MemoryStream
    {
        public override String ToString()
        {
            return Encoding.Default.GetString(base.ToArray());
        }

        public void WriteUTF(String paramString)
        {
            if (paramString == null)
                paramString = String.Empty;

            int strlen = paramString.Length;
            int utflen = 0;
            int c, count = 0;

            for (int i = 0; i < strlen; i++)
            {
                c = paramString[i];
                if ((c >= 0x0001) && (c <= 0x007F))
                {
                    utflen++;
                }
                else if (c > 0x07FF)
                {
                    utflen += 3;
                }
                else
                {
                    utflen += 2;
                }
            }

            if (utflen > 65535)
                throw new Exception("Encoded string too long: " + utflen + " bytes");
            byte[] bytearr = new byte[utflen + 2];
            bytearr[count++] = (byte)((utflen >> 8) & 0xFF);
            bytearr[count++] = (byte)((utflen >> 0) & 0xFF);

            int i1 = 0;
            for (; i1 < strlen; i1++)
            {
                c = paramString[i1];
                if (!((c >= 0x0001) && (c <= 0x007F))) break;
                bytearr[count++] = (byte)c;
            }

            for (; i1 < strlen; i1++)
            {
                c = paramString[i1];
                if ((c >= 0x0001) && (c <= 0x007F))
                {
                    bytearr[count++] = (byte)c;

                }
                else if (c > 0x07FF)
                {
                    bytearr[count++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
                    bytearr[count++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    bytearr[count++] = (byte)(0x80 | ((c >> 0) & 0x3F));
                }
                else
                {
                    bytearr[count++] = (byte)(0xC0 | ((c >> 6) & 0x1F));
                    bytearr[count++] = (byte)(0x80 | ((c >> 0) & 0x3F));
                }
            }

            Write(bytearr, 0, utflen + 2);
        }

        public void Write(int value)
        {
            WriteByte((byte)value);
        }

        public void WriteChars(string value)
        {
            int len = value.Length;

            for (int i = 0; i < len; i++)
                WriteChar((int)value[i]);
        }

        public void WriteShort(int value)
        {
            Write(value >> 8);
            Write(value);
        }

        public void WriteChar(int value)
        {
            Write((char)value);
        }

        public void WriteInt(int value)
        {
            Write(value >> 24);
            Write(value >> 16);
            Write(value >> 8);
            Write(value);
        }

        public void WriteLong(long value)
        {
            Write((byte)(value >> 56));
            Write((byte)(value >> 48));
            Write((byte)(value >> 40));
            Write((byte)(value >> 32));
            Write((byte)(value >> 24));
            Write((byte)(value >> 16));
            Write((byte)(value >> 8));
            Write((byte)value);
        }

        public void WriteFloat(float value)
        {
            WriteInt(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
        }

        public void WriteDouble(double value)
        {
            WriteLong(BitConverter.DoubleToInt64Bits(value));
        }

        public void WriteBytes(String value)
        {
            int len = value.Length;

            for (int i = 0; i < len; i++)
                Write((byte)value[i]);
        }

        public void WriteBoolean(bool value)
        {
            Write(value ? 1 : 0);
        }
    }
}
