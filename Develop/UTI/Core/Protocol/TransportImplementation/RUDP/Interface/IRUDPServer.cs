using System;
using System.Collections.Generic;
using System.Net;

namespace UTI
{
    public interface IRUDPServer
    {
        event OnReceiveMessage_delegate OnReceiveClientMessage;
        event OnReceiveMessage_Bytes_delegate OnReceiveClientMessage_Bytes;
        event OnConnected_delegate OnClientConnectedEvent;
        event OnDisconnected_delegate OnClientDisconnectedEvent;

        void BeginServer();
        void DisconnectClient(IConnection conn);
        List<IConnection> GetClients();
        bool IsConnectedClient(IPEndPoint ip);
        bool IsConnectedClient(IPEndPoint ip, out IConnection client);
        void SendMessageToClient(IConnection client, byte mark, string message, object[] objects);
        void SendMessageToClient_Bytes(IConnection client, byte mark, string message, IBytesFormater[] objects);
        void StopServer();
    }
}