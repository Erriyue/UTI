using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace UTI
{
    public static class SocketManager
    {
        public static Socket UDPSocket(IPEndPoint ip)
        {
            var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

            //禁用10054报错崩溃
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, new byte[4]);

            Socket.Bind(ip);
            return Socket;
        }
    }
}


