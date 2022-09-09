using System;
using System.Net;
using System.Threading.Tasks;

namespace UTI
{

    public interface IRUDPClient
    {
        event OnReceiveMessage_delegate OnReceiveServerMessage;
        event OnReceiveMessage_Bytes_delegate OnReceiveServerMessage_Bytes;
        event OnConnected_delegate OnServerConnectedEvent;
        event OnDisconnected_delegate OnServerDisconnectedEvent;

        void BeginClient();
        Task<ConnectResult> ConnectServer(IPEndPoint ip);
        void DisconnectServer(IPEndPoint ip);
        IConnection GetServer();
        bool IsConnectedServer(IPEndPoint ip);
        bool IsConnectedServer(IPEndPoint ip, out IConnection client);
        void SendMessageToServer(byte mark, string message, object[] objects);
        void SendMessageToServer_Bytes(byte mark, string message, IBytesFormater[] objects);
        void StopClient();
    }
}