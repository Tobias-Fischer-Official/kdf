using System;
using System.Text;
using System.IO;

namespace KDF.Networks.Protocol.Module
{
    internal class PacketReader : MemoryStream
    {
        public PacketReader(byte[] buffer)
            : base(buffer)
        {
        }

        public String ReadUTF()
        {
            return ReadString();
        }

        public void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }

        public void ReadFully(byte[] b, int off, int len)
        {
            int n = 0;
            while (n < len)
            {
                int count = Read(b, off + n, len - n);
                if (count < 0)
                    throw new Exception("EOFException");
                n += count;
            }
        }

        public byte Read()
        {
            return (byte)ReadByte();
        }

        public Boolean ReadBoolean()
        {
            return Read() != 0;
        }

        public long ReadLong()
        {
            return (((long)ReadByte() & 0xFF << 56) +
                    ((long)(ReadByte() & 0xFF) << 48) +
                    ((long)(ReadByte() & 0xFF) << 40) +
                    ((long)(ReadByte() & 0xFF) << 32) +
                    ((long)(ReadByte() & 0xFF) << 24) +
                    ((ReadByte() & 0xFF) << 16) +
                    ((ReadByte() & 0xFF) << 8) +
                    ((ReadByte() & 0xFF) << 0));
        }

        public short ReadShort()
        {
            return (short)(((Read() & 0xFF) << 8)
                    + (Read() & 0xFF));
        }

        public int ReadInt()
        {
            return ((Read() & 0xFF) << 24)
                    + ((Read() & 0xFF) << 16)
                    + ((Read() & 0xFF) << 8)
                    + (Read() & 0xFF);
        }

        public char ReadChar()
        {
            return (char)((Read() << 8) + (Read() << 0));
        }

        public String ReadString()
        {
            int len = ReadShort();

            StringBuilder builder = new StringBuilder(len);
            for (int i = 0; i < len; i++)
                builder.Append((char)Read());

            return builder.ToString();
        }
    }
}
