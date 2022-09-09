using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UTI;
using System.Threading.Tasks;

namespace UTItest.Transport.Bean
{
    public class RUDPTransportType : ITransportType
    {
        public byte Type { get; }
        public int Order { get; }
        public TransportDatagramType TypeEnum => (TransportDatagramType)Type;

        public RUDPTransportType(TransportDatagramType type, int order)
        {
            Type = (byte)type;
            Order = order;
        }
        public RUDPTransportType(byte type, int order)
        {
            Type = (byte)type;
            Order = order;
        }
    }

    public class RUDPTransportModel 
    {
        public IPEndPoint IP { get; }
        public List<RUDPClient> Connections { get; } = new List<RUDPClient>();
        public RUDPTransportProtocol Transport { get; }
        private INetStateControler State { get; }

        public RUDPTransportModel(IPEndPoint iP)
        {
            this.IP = iP;
            this.Connections = new List<RUDPClient>();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            socket.Bind(this.IP);

            Transport = new RUDPTransportProtocol(this.IP, socket);
            State = new NetStateControler(new UninitializedClient(Transport, State));
        }

    }

    public class RUDPCore : INetTransport
    {
        public RUDPTransportModel Model;
        private ITransportProtocol Protocol => Model.Transport;
        private List<(IPEndPoint)>

        public RUDPCore(RUDPTransportModel model)
        {
            this.Model = model; 
            Task.Run(Sending);
        }

        private async void Sending()
        {
            Stopwatch ResendWatch = new Stopwatch();
            ResendWatch.Restart();

            while (true)
            {
                await Task.Delay(10);

                Model.Connections.RemoveAll(e => e.IsTimeOut());

                foreach (var client in Model.Connections.ToArray())
                {
                    var first = client.GetFirst();
                    if (first != null && ResendWatch.ElapsedMilliseconds > 100)
                    {
                        ResendWatch.Restart();
                        Protocol.Send(client.IP, first.Message, first.Type);
                    }
                }
            }
        }



        public event Action<IPEndPoint> OnBeConnected;
        public event Action<IPEndPoint> OnBeDisconnect;
        public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;
        public async Task<ConnectionResult> Connect(IPEndPoint ip)
        {
            Model.Connections.Add(new RUDPClient(ip));
            PushSendList(ip,"", new RUDPTransportType(TransportDatagramType.Connect,0));//消息这里未来可以写一些客户端的什么个人信息之类的
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100 * 10);
            }
            return new ConnectionResult(false,"Time Out");
        }
        public Task<ITransportResult> Send(IPEndPoint ip, string message, ITransportType type)
        {

        }
        public Task<ConnectionResult> Disconnect(IPEndPoint ip)
        {

        }





        private void OnReceivedMessage_(IPEndPoint senderIp, string message, ITransportType TypeClass)
        {
            TransportDatagramType type = (TypeClass as RUDPTransportType).TypeEnum;
            int Order = OrderClass.Order;

            switch (type)
            {
                case TransportDatagramType.Connect:
                    if (!isRunning)
                        Log.Error("服务器关闭过程中收到一个连接请求,已被抛弃");
                    if (isClient)
                        Log.Error("错误!客户端不能连接客户端");
                    else if (isServer)
                    {
                        if (ConnectedConnection.ContainsKey(senderIP))
                        {
                            return;
                        }
                        RUDPClient client = new RUDPClient(senderIP, this);
                        ConnectedConnection.Add(senderIP, client);

                        SendDirect(client.IPEP.ToString(), (byte)TransportDatagramType.Acknowledge, "");
                    }
                    break;
                case TransportDatagramType.Data:
                    ConnectedConnection[senderIP].UpdateLastedMessage(Order, type, message);
                    SendDirect(senderIP.ToString(), (byte)TransportDatagramType.Acknowledge, "", Order);
                    break;
                case TransportDatagramType.Acknowledge:
                    ConnectedConnection[senderIP].AcknowledgeMessage(Order);
                    if (isClient)
                    {
                        SendDirect(senderIP.ToString(), (byte)TransportDatagramType.Acknowledge, "", Order);
                    }
                    break;
            }
        }






        /// <summary>
        /// 将需要发送的安全型信息封装到发送列表里
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="_message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ITransportResult PushSendList(IPEndPoint ip, string message, RUDPTransportType type)
        {
            var Conn = Model.Connections.Find(e => e.IP == ip);
            if (Conn == null)
            {
                return new TransportResult(TransportResultType.ConnectionNotExist, "ConnectionNotExist");
            }

            //推送消息
            Conn.Push(new TransportVirtualPackage(Conn.Order, message, new RUDPTransportType(type.Type,Conn.Order)));
            return new TransportResult(TransportResultType.Successed, "Pushed");
        }
        /// <summary>
        /// 直接发送需要发送的内容
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ITransportResult SendDirect(IPEndPoint IP, string message,ITransportType type)
        {
            return Protocol.Send(IP, message, type);
        }







        public void ComplyMessage(RUDPClient client, ProtocolPackage package)
        {
            if (ConnectedConnection.ContainsKey(client.IPEP))
            {
                OnReceived?.Invoke(client.IPEP.ToString(), (byte)package.Type, package.Message);
                SendDirect(client.IPEP.ToString(), (byte)TransportDatagramType.Acknowledge, "");
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

            foreach (var v in ConnectedConnection)
                Send(v.Key.ToString(), (byte)TransportDatagramType.Disconnect, "");
            //虽然用不到连接请求队列,但还是清空了保险一些吧
            PendingConnection.Clear();
            ConnectedConnection.Clear();

            isServer = false;
        }



    }
}


