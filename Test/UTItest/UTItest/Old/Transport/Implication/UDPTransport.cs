using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace UTI
{
    class UDPTransport : TransportBase, ITransportBasic
    {
        private Action<string, byte, string> m_OnReceived;

        bool ITransportBasic.IsRunning { get => isRunning; set => isRunning = value; }
        bool ITransportBasic.IsClient { get => isClient; set => isClient = value; }
        bool ITransportBasic.IsServer { get => isServer; set => isServer = value; }
        public Action<string, byte, string> OnReceived { get => m_OnReceived; set => m_OnReceived = value; }

        public UDPTransport() : base()
        {
            //因为是UDP,所以不需要赋值IP地址
            Task.Run(Run);
        }
        public UDPTransport(string _ip, int _port) : base(_ip,_port)
        {
            Task.Run(Run);
        }

        async void Run()
        {
            SocketReceiveMessageFromResult _result;

            while (isRunning)
            {
                //1.消息处理
                _result = await m_socket.ReceiveMessageFromAsync(m_buffer, SocketFlags.None, m_IPEP);

                string message = Encoding.UTF8.GetString(m_buffer, 0, _result.ReceivedBytes - 1);
                TransportDatagramType type = (TransportDatagramType)m_buffer[_result.ReceivedBytes - 1];
                string sender = _result.RemoteEndPoint.ToString();

                switch (type)
                {
                    case TransportDatagramType.Connect:
                        if (!isRunning)
                            Log.Warning("服务器关闭过程中收到一个连接请求,已被抛弃");
                        if (isClient)
                            Log.Error("错误!客户端不能连接客户端");
                        else if (isServer)
                        {
                            Log.Server($"收到一个来自{sender}的连接请求");
                            var ip = (IPEndPoint)_result.RemoteEndPoint;
                            //UDP不需要Pending因为UDP能直接发送报文过来,Pending是对于TCP来说,TCP需要先连接握手请求,然后才能发送数据.
                            //当然这也是为什么DDoS对于TCP的破坏力比UDP大得多
                            if (ConnectedConnection.ContainsKey(ip))
                            {
                                Log.Error("错误!一个客户端不能连接服务器两次");
                                return;
                            }
                            Client client = new Client(ip);
                            ConnectedConnection.Add(ip, client);
                        }
                        break;
                    case TransportDatagramType.Data:
                        Log.Server($"{sender}:{message}(消息类型:{type})");
                        OnReceived?.Invoke(sender, (byte)type, message);
                        break;
                }
                //2.连接检查,将所有挂起的连接Pending全部转正,并将所有超时的连接全部剔除
                for (int i = 0; i < ConnectedConnection.Count; i++)
                {
                    if (ConnectedConnection.ElementAt(i).Value.isTimeout())
                    {
                        ConnectedConnection.Remove(ConnectedConnection.ElementAt(i).Key);
                        i--;
                    }
                }
                //3.查看是否本传送层是否不再被需要
                //TODO未来这里应该会有一个销毁方法,这里只提供一些可能的情况
                //if(isClient && ConnectedConnection.Count == 0)
                //if(isServer && ShuttingDown)
            }
        }




        public int Send(string _IP, byte _type, string _message)
        {
            IPEndPoint ip = IPEndPoint.Parse(_IP);
            byte[] context = Encoding.UTF8.GetBytes(_message);
            if (context.Length > BufferSize - 1)
                return (int)TransportResultType.OutOfBoundry;
            byte[] message = context.Concat(new byte[] { _type }).ToArray();

            if (ConnectedConnection.ContainsKey(ip))
                m_socket.SendTo(message, ip);
            else
                return (int)TransportResultType.ConnectionNotExist;
            return (int)TransportResultType.Successed;
        }

        public void Connect(string ip)
        {
            if (!isClient && !isServer)
            {
                //UDP无需连接握手协议,直接将服务器ip标记即可
                isClient = true;
                isRunning = true;

                IPEndPoint ServerIP = IPEndPoint.Parse(ip);
                ConnectedConnection.Add(ServerIP, new Client(ServerIP));

                Send(ServerIP.ToString(), (byte)TransportDatagramType.Connect, "");//消息这里未来可以写一些客户端的什么个人信息之类的
            }
            else if (isClient)
            {
                Log.Error("您已经连接到一个服务器了,不能再连接第二个");
            }
            else if (isServer)
            {
                Log.Error("错误!服务器之间不能互相连接");
            }
        }
        public void Disconnect()
        {
            if (!isClient)
            {
                Log.Error("您还没有连接,不能断开无连接的连接,或者您是服务器,请使用ShutdownServer关闭服务器");
                return;
            }
            isClient = false;

            Send(ServerIP.ToString(), (byte)TransportDatagramType.Disconnect, "");//消息里未来可以写一些什么断开原因之类的
        }

        public void SetupServer()
        {
            isServer = true;
            isRunning = true;
            //UDP不需要建立监听,在初始化里初始化套接字完毕后别人发过来的报文直接就能解析
        }

        public void ShutdownServer()
        {
            isRunning = false;

            foreach(var v in ConnectedConnection)
                Send(v.Key.ToString(), (byte)TransportDatagramType.Disconnect, "");
            //虽然用不到连接请求队列,但还是清空了保险一些吧
            PendingConnection.Clear();
            ConnectedConnection.Clear();

            isServer = false;
        }
    }
}
