using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UTI
{
    public delegate void OnReceiveMessage_delegate(IConnection conn, byte mark, string message, object[] objects);
    public delegate void OnReceiveMessage_Bytes_delegate(IConnection conn, byte mark, string message, IBytesFormater[] objects);
    public delegate void OnConnected_delegate(IConnection conn);
    public delegate void OnDisconnected_delegate(IConnection conn);

    public static class RUDP
    {
        public static IObjectConverter DefaultConverter = new JsonObjectConverter();

        public static IRUDPClient StartClient(IPEndPoint ip)
        {
            IRUDPClient client = new RUDPClient(ip, DefaultConverter);
            client.BeginClient();
            return client;
        }
        public static IRUDPServer StartServer(IPEndPoint ip)
        {
            IRUDPServer server = new RUDPServer(ip, DefaultConverter);
            server.BeginServer();
            return server;
        }
    }
}


