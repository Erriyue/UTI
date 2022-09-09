using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace UTI
{
    public class RUDPServer : UniversalTransport, IRUDPServer
    {
        public IPEndPoint IP { get; }
        public RUDPServer(IPEndPoint ip, IObjectConverter converter) : base(SocketManager.UDPSocket(ip), converter)
        {
            this.IP = ip;
        }



        /// <summary>
        /// 开始服务
        /// </summary>
        public void BeginServer()
        {
            this.BeginListening(this.IP);
            this.BeginReliableRepeat();
            this.BeginConnection();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            this.StopConnection();
            this.StopReliableRepeat();
            this.StopListening();
        }




        /// <summary>
        /// 获取所有连接的客户端
        /// </summary>
        /// <returns></returns>
        public List<IConnection> GetClients()
        {
            List<IConnection> list = new List<IConnection>(this.Connections.Values.ToList());
            list.RemoveAll(e => (!IsConnectedClient(e.IP)));
            return list;
        }
        /// <summary>
        /// 是本服务器的客户端?
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsConnectedClient(IPEndPoint ip)
        {
            if (base.IsConnected(ip, out var conn))
            {
                if (!conn.IsServer)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsConnectedClient(IPEndPoint ip, out IConnection client)
        {
            if (base.IsConnected(ip, out var conn))
            {
                if (!conn.IsServer)
                {
                    client = conn;
                    return true;
                }
            }
            client = null;
            return false;
        }



        protected override void OnClientConnected(Connection conn)
        {
            OnClientConnectedEvent?.Invoke(conn);
        }
        /// <summary>
        /// 客户端连接
        /// </summary>
        public event OnConnected_delegate OnClientConnectedEvent;
        protected override void OnClientDisconnected(Connection conn)
        {
            OnClientDisconnectedEvent?.Invoke(conn);
        }
        /// <summary>
        /// 客户端失去连接
        /// </summary>
        public event OnDisconnected_delegate OnClientDisconnectedEvent;




        /// <summary>
        /// 关闭和客户端的连接
        /// </summary>
        /// <param name="conn"></param>
        public void DisconnectClient(IConnection conn)
        {
            if (IsConnectedClient(conn.IP, out var client))
            {
                base.TryDisconnect(client.IP);
            }
        }




        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息</param>
        /// <param name="objects">可Json化的对象</param>
        public void SendMessageToClient(IConnection client, byte mark, string message, object[] objects)
        {
            base.SendMessage(client.IP, mark, message, objects);
        }
        /// <summary>
        /// 向客户端发送消息 (字节格式对象)
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息</param>
        /// <param name="objects">实现字节格式接口的对象</param>
        public void SendMessageToClient_Bytes(IConnection client, byte mark, string message, IBytesFormater[] objects)
        {
            base.SendMessage(client.IP, mark, message, objects);
        }



        protected override void OnReceiveMessage(IPEndPoint ip, byte mark, string message, object[] objects)
        {
            if (IsConnectedClient(ip, out var client))
            {
                OnReceiveClientMessage?.Invoke(client, mark, message, objects);
            }
        }
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        public event OnReceiveMessage_delegate OnReceiveClientMessage;
        protected override void OnReceiveMessage(IPEndPoint ip, byte mark, string message, IBytesFormater[] objects)
        {
            if (IsConnectedClient(ip, out var client))
            {
                OnReceiveClientMessage_Bytes?.Invoke(client, mark, message, objects);
            }
        }
        /// <summary>
        /// 接收客户端消息 (字节格式对象)
        /// </summary>
        public event OnReceiveMessage_Bytes_delegate OnReceiveClientMessage_Bytes;
    }
}


