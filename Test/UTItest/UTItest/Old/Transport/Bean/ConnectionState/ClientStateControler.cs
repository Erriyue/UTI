using System;
using System.Net;
using System.Threading.Tasks;

namespace UTI
{
    public class ConnectionResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public ConnectionResult(bool successed, string message)
        {
            IsSuccess = successed;
            Message = message;
        }
    }
    public class RunningResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public RunningResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }



    public interface INetTransport
    {
        Task<ConnectionResult> Connect(IPEndPoint ip);
        //被连接
        event Action<IPEndPoint> OnBeConnected;
        //被对方发送
        public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;
        //发送给对方
        public Task<ITransportResult> Send(IPEndPoint ip, string message, ITransportType type);
        //关闭连接
        Task<ConnectionResult> Disconnect(IPEndPoint ip);
        //被对方关闭连接
        event Action<IPEndPoint> OnBeDisconnect;

    }
    public interface INetworkState : INetTransport
    {
        //开始服务
        Task<RunningResult> Start();
        //连接对方
        //释放资源
        Task<RunningResult> Dispose();
    }
    public interface INetworkStateClient
    {

    }
    public interface INetworkStateServer
    {

    }




    public interface INetClientState : INetworkState
    {
        INetStateControler Controler { get; }
    }
    public interface INetStateControler : INetworkState
    {
        INetClientState State { get; }
        void __ChangeState__(INetClientState state);
    }




    //public class UninitlizedConnection : INetClientState
    //{
    //    public async Task<ConnectionResult> Connect(IPEndPoint ip)
    //    {

    //    }

    //    public Task<ConnectionResult> Disconnect(IPEndPoint ip)
    //    {
    //        return Task.FromResult(new ConnectionResult(false, "UninitlizedConnection"));
    //    }
    //}
    //public class ConnectedConnection : INetClientState
    //{

    //}
    //public class DisconnectedConnection : INetClientState
    //{

    //}

}
