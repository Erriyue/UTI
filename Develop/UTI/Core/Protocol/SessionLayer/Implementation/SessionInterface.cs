using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;


namespace UTI
{
    public class SessionInterface : AsyncReliableTransport
    {
        public SessionInterface(Socket socket, int bufferSize) : base(socket, bufferSize)
        {

        }
        public const int MaxConnectionCount = 500;
        public const int MaxPing = 1000;
        public const int DelayMs = 5000;
        public const int ClientMaxTimeOutCount = 3;
        public const int ClientMaxCheckDelay = 6000;
        public const int ClientMaxCheckDelayCount = 60;
        public const int ConnectionAsyncDelay = 10;
        public ConcurrentDictionary<IPEndPoint, Connection> Connections { get; } = new ConcurrentDictionary<IPEndPoint, Connection>();
        private ConcurrentDictionary<IPEndPoint, bool> TryConnectList { get; } = new ConcurrentDictionary<IPEndPoint, bool>();
        private bool IsConnectionCheck;
        private Task ConnectionCheckTask;


        public void BeginConnection()
        {
            if (IsConnectionCheck) return;
            IsConnectionCheck = true;
            this.Connections.Clear();
            ConnectionCheckTask = Task.Run(ConnectionCheck);
        }
        public void StopConnection()
        {
            if (!IsConnectionCheck) return;
            IsConnectionCheck = false;
            ConnectionCheckTask.Dispose();
            ConnectionCheckTask = null;
            this.Connections.Clear();
        }
        private async void ConnectionCheck()
        {
            while (IsConnectionCheck)
            {
                await Task.Delay(DelayMs);
                foreach (var item in Connections.ToArray())
                {
                    item.Value.BeginCheck();
                    SendSessionInfo(item.Key, SessionType.Check);
                }
                await Task.Delay(MaxPing);
                foreach (var item in Connections.ToArray())
                {
                    if (item.Value.IsTimeOut())
                    {
                        OnDisconnect(item.Key);
                        SendSessionInfo(item.Key, SessionType.TimeOutDisconnect);
                    }
                }
            }
        }


        public async Task<ConnectResult> TryConnect(IPEndPoint ip)
        {
            if (Connections.Count >= MaxConnectionCount)
            {
                return ConnectResult.TooMany;
            }
            if (IsConnected(ip))
            {
                return ConnectResult.AlradyExisit;
            }
            if (TryConnectList.ContainsKey(ip))
            {
                return ConnectResult.WaitForConnecting;
            }
            else
            {
                TryConnectList.TryAdd(ip, true);
            }

            this.SendSessionInfo(ip, SessionType.CheckIfConnectable);
            int timeout = ClientMaxCheckDelayCount;
            while (timeout > 0)
            {
                if (IsConnected(ip))
                {
                    TryConnectList.TryRemove(ip, out var v2);
                    return ConnectResult.Successed;
                }
                timeout--;
                await Task.Delay(ConnectionAsyncDelay);
            }

            if (Connections.ContainsKey(ip))
            {
                Connections.TryRemove(ip, out var value);
            }
            this.SendSessionInfo(ip, SessionType.Disconnect);
            TryConnectList.TryRemove(ip, out var v);
            return ConnectResult.TimeOut;
        }
        public void TryDisconnect(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                bool connected = false;
                if (IsConnected(ip))
                {
                    connected = true;
                }

                if (Connections.TryRemove(ip, out var value))
                {
                    this.SendSessionInfo(ip, SessionType.Disconnect);
                    if (connected)
                    {
                        OnDisconnected(value);
                    }
                }
            }
        }



        protected bool IsConnected(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                if (conn.State == ConnectionState.Connected)
                {
                    return true;
                }
            }
            return false;
        }
        protected bool IsConnected(IPEndPoint ip, out Connection connection)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                if (conn.State == ConnectionState.Connected)
                {
                    connection = conn;
                    return true;
                }
            }
            connection = null;
            return false;
        }



        private void OnConnected(Connection conn)
        {
            if (conn.IsServer)
            {
                OnServerConnected(conn);
            }
            else
            {
                OnClientConnected(conn);
            }
        }
        protected virtual void OnServerConnected(Connection conn)
        {

        }
        protected virtual void OnClientConnected(Connection conn)
        {

        }
        private void OnDisconnected(Connection conn)
        {
            if (conn.IsServer)
            {
                OnServerDisconnected(conn);
            }
            else
            {
                OnClientDisconnected(conn);
            }
        }
        protected virtual void OnServerDisconnected(Connection conn)
        {

        }
        protected virtual void OnClientDisconnected(Connection conn)
        {

        }




        protected sealed override void OnReceivedFormat_(IPEndPoint ip, IBytesFormater[] message)
        {
            var data = DeFormat(message);
            if (data.Flag != null)
            {
                if (data.Flag.DataType == SessionType.CheckIfConnectable)
                {
                    Server_OnCheckIfConnect(ip);
                }
                else if (data.Flag.DataType == SessionType.Initilize)
                {
                    Client_OnInitilize(ip);
                }
                else if (data.Flag.DataType == SessionType.Connect)
                {
                    Server_OnConnect(ip);
                }
                else if (data.Flag.DataType == SessionType.Connecting)
                {
                    Client_OnConnecting(ip);
                }
                else if (data.Flag.DataType == SessionType.Connected)
                {
                    Server_OnConnected(ip);
                }
                else if (data.Flag.DataType == SessionType.Disconnect)
                {
                    OnDisconnect(ip);
                }
                else if (data.Flag.DataType == SessionType.ErrorDisconnect)
                {
                    OnDisconnectError(ip);
                }
                else if (data.Flag.DataType == SessionType.Check)
                {
                    OnCheck(ip);
                }
                else if (data.Flag.DataType == SessionType.CheckBack)
                {
                    OnCheckBack(ip);
                }
                else if (data.Flag.DataType == SessionType.TimeOutDisconnect)
                {
                    OnDisconnect(ip);
                }
            }
            else
            {
                OnReceivedFormat__(ip, message);
            }
        }
        protected virtual void OnReceivedFormat__(IPEndPoint ip, IBytesFormater[] message)
        {

        }



        protected virtual void Server_OnCheckIfConnect(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                return;
            }
            if (Connections.Count >= MaxConnectionCount)
            {
                return;
            }
            var conn = CreateClientConnection(ip);
            if (Connections.TryAdd(ip, conn))
            {
                conn.BeginCheck();
                SendSessionInfo(ip, SessionType.Initilize);
            }
        }
        private void Server_OnConnect(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                conn.CheckedCallBack();
                if (conn.State == ConnectionState.Init)
                {
                    conn.SetState(ConnectionState.Connecting);
                    SendSessionInfo(ip, SessionType.Connecting);
                }
            }
        }
        protected virtual void Server_OnConnected(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                if (conn.State == ConnectionState.Connecting)
                {
                    conn.SetState(ConnectionState.Connected);
                    OnConnected(conn);
                }
            }
        }
        private void Client_OnInitilize(IPEndPoint ip)
        {
            var conn = CreateServerConnection(ip);
            if (Connections.TryAdd(ip, conn))
            {
                conn.BeginCheck();
                conn.SetState(ConnectionState.Connecting);
                SendSessionInfo(ip, SessionType.Connect);
            }
        }
        protected virtual void Client_OnConnecting(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                conn.CheckedCallBack();
                if (conn.State == ConnectionState.Connecting)
                {
                    conn.SetState(ConnectionState.Connected);
                    SendSessionInfo(ip, SessionType.Connected);
                    OnConnected(conn);
                }
            }
        }


        private void OnDisconnect(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                bool connected = false;
                if (IsConnected(ip))
                {
                    connected = true;
                }

                if (Connections.TryRemove(ip, out var value))
                {
                    if (connected)
                    {
                        OnDisconnected(value);
                    }
                }
            }
        }
        private void OnDisconnectError(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                bool connected = false;
                if (IsConnected(ip))
                {
                    connected = true;
                }
                if (Connections.TryRemove(ip, out var value))
                {
                    this.SendSessionInfo(ip, SessionType.Disconnect);
                    if (connected)
                    {
                        OnDisconnected(value);
                    }
                }
            }
        }
        private void OnCheck(IPEndPoint ip)
        {
            SendSessionInfo(ip, SessionType.CheckBack);
        }
        private void OnCheckBack(IPEndPoint ip)
        {
            if (Connections.ContainsKey(ip))
            {
                var conn = Connections[ip];
                conn.CheckedCallBack();
            }
        }
        private void SendSessionInfo(IPEndPoint ip, SessionType type)
        {
            base.SendFormat(ip, Format(new SessionFlag() { DataType = type }, new IBytesFormater[0]));
        }




        protected virtual Connection CreateServerConnection(IPEndPoint ip)
        {
            var conn = new Connection(ip);
            conn.Init(new ConnectionRule(ClientMaxTimeOutCount, MaxPing, ClientMaxCheckDelay), new ConnectionSetting(true));
            return conn;
        }
        protected virtual Connection CreateClientConnection(IPEndPoint ip)
        {
            var conn = new Connection(ip);
            conn.Init(new ConnectionRule(ClientMaxTimeOutCount, MaxPing, ClientMaxCheckDelay), new ConnectionSetting(false));
            return conn;
        }






        private IBytesFormater[] Format(SessionFlag Flag, IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is SessionFlag)
            {
                var flag = message[0] as SessionFlag;
                flag.DataType = Flag.DataType;

                return message;
            }
            else
            {
                var flag = new SessionFlag();
                flag.DataType = Flag.DataType;

                var messageList = new List<IBytesFormater>() { flag };
                messageList.AddRange(message);
                return messageList.ToArray();
            }
        }
        private (SessionFlag Flag, IBytesFormater[] message) DeFormat(IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is SessionFlag)
            {
                var flag = message[0] as SessionFlag;
                return (flag, message.Skip(1).ToArray());
            }
            else
            {
                return (null, message);
            }
        }



        #region OnSend
        //protected sealed override void OnSendFormat_Raw(int sendedFormatLength, IPEndPoint ip, ITransportBytesFormat[] message)
        //{
        //    var data = DeFormat(message);
        //    if (data.Flag != null)
        //    {
        //        OnSendMessage(ip, data.message);
        //    }
        //    else
        //    {
        //        OnSendFormat_Raw2(sendedFormatLength, ip, message);
        //    }
        //}
        //protected virtual void OnSendFormat_Raw2(int sendedFormatLength, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        //protected virtual void OnSendMessage(IPEndPoint ip, ITransportBytesFormat[] item)
        //{

        //}
        #endregion
    }
}


