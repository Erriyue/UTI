using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UTI
{
    public class RUDPClient : UniversalTransport, IRUDPClient
    {
        public IPEndPoint IP { get; }
        public RUDPClient(IPEndPoint ip, IObjectConverter converter) : base(SocketManager.UDPSocket(ip), converter)
        {
            this.IP = ip;
        }

        protected override void Server_OnCheckIfConnect(IPEndPoint ip)
        {
            //不可被连接
        }


        /// <summary>
        /// 开始服务
        /// </summary>
        public void BeginClient()
        {
            this.BeginListening(this.IP);
            this.BeginReliableRepeat();
            this.BeginConnection();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopClient()
        {
            this.StopConnection();
            this.StopReliableRepeat();
            this.StopListening();
        }





        /// <summary>
        /// 尝试连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ConnectResult> ConnectServer(IPEndPoint ip)
        {
            if (GetServer() != null)
            {
                return ConnectResult.TooMany;
            }
            if (IsConnected(ip, out var conn))
            {
                if (conn.IsServer)
                {
                    return ConnectResult.AlradyExisit;
                }
                else
                {
                    return ConnectResult.Failed;
                }
            }
            return await base.TryConnect(ip);
        }
        /// <summary>
        /// 关闭服务器连接
        /// </summary>
        /// <param name="ip"></param>
        public void DisconnectServer(IPEndPoint ip)
        {
            base.TryDisconnect(ip);
        }




        /// <summary>
        /// 获取连接的服务器, 没有时返回null
        /// </summary>
        /// <returns></returns>
        public IConnection GetServer()
        {
            var list = this.Connections.Values.ToList();
            list.RemoveAll(e => (!IsConnectedServer(e.IP)));
            return list.Count > 0 ? list[0] : null;
        }
        /// <summary>
        /// 是本客户端连接的服务器?
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsConnectedServer(IPEndPoint ip)
        {
            if (base.IsConnected(ip, out var conn))
            {
                if (conn.IsServer)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsConnectedServer(IPEndPoint ip, out IConnection client)
        {
            if (base.IsConnected(ip, out var conn))
            {
                if (conn.IsServer)
                {
                    client = conn;
                    return true;
                }
            }
            client = null;
            return false;
        }




        protected override void OnServerConnected(Connection conn)
        {
            OnServerConnectedEvent?.Invoke(conn);
        }
        /// <summary>
        /// 连接到服务器
        /// </summary>
        public event OnConnected_delegate OnServerConnectedEvent;
        protected override void OnServerDisconnected(Connection conn)
        {
            OnServerDisconnectedEvent?.Invoke(conn);
        }
        /// <summary>
        /// 与服务器失去连接
        /// </summary>
        public event OnDisconnected_delegate OnServerDisconnectedEvent;



        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息</param>
        /// <param name="objects">可Json化的对象</param>
        public void SendMessageToServer(byte mark, string message, object[] objects)
        {
            var server = GetServer();
            if (server != null)
            {
                base.SendMessage(server.IP, mark, message, objects);
            }
        }
        /// <summary>
        /// 向服务器发送消息 (字节格式对象)
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息</param>
        /// <param name="objects">实现字节格式接口的对象</param>
        public void SendMessageToServer_Bytes(byte mark, string message, IBytesFormater[] objects)
        {
            var server = GetServer();
            if (server != null)
            {
                base.SendMessage(server.IP, mark, message, objects);
            }
        }



        protected override void OnReceiveMessage(IPEndPoint ip, byte mark, string message, object[] objects)
        {
            if (IsConnectedServer(ip, out var client))
            {
                OnReceiveServerMessage?.Invoke(client, mark, message, objects);
            }
        }
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        public event OnReceiveMessage_delegate OnReceiveServerMessage;
        protected override void OnReceiveMessage(IPEndPoint ip, byte mark, string message, IBytesFormater[] objects)
        {
            if (IsConnectedServer(ip, out var client))
            {
                OnReceiveServerMessage_Bytes?.Invoke(client, mark, message, objects);
            }
        }
        /// <summary>
        /// 接收客户端消息 (字节格式对象)
        /// </summary>
        public event OnReceiveMessage_Bytes_delegate OnReceiveServerMessage_Bytes;
    }
}


