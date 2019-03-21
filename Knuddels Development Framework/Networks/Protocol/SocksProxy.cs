using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace KDF.Networks.Protocol
{
    internal class SocksProxy
    {
        public static void EstablishConnection(Socket socket, string host, int port)
        {
            TcpClient client = new TcpClient();
            client.Client = socket;
            byte[] bytes = GetAuthByteArray();
            client.GetStream().Write(bytes, 0, bytes.Length);
            client.GetStream().Flush();
            GetConnectionReply(client);

            bytes = GetConnectionByteArray(host, port);
            client.GetStream().Write(bytes, 0, bytes.Length);
            client.GetStream().Flush();
            GetConnectionReply(client);
        }

        private static byte[] GetAuthByteArray()
        {
            MemoryStream stream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((byte)5); // socks version
                writer.Write((byte)1); // method count
                writer.Write((byte)0); // method type
            }

            return stream.ToArray();
        }

        private static byte[] GetAddressByteArray(string host)
        {
            IPAddress address;

            try
            {
                address = IPAddress.Parse(host);
            }
            catch
            {
                address = Dns.GetHostEntry(host).AddressList[0];
            }

            return address.GetAddressBytes();
        }

        private static byte[] GetConnectionByteArray(string host, int port)
        {
            MemoryStream stream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((byte)5);
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write((byte)1);
                writer.Write(GetAddressByteArray(host));
                writer.Write((byte)((port >> 8) & 0xFF));
                writer.Write((byte)(port & 0xFF));
            }

            return stream.ToArray();
        }

        private static void GetConnectionReply(TcpClient client)
        {
            byte[] buffer = new byte[1024];
            int count = client.GetStream().Read(buffer, 0, buffer.Length);
            // Implement error code returning.
        }
    }
}
