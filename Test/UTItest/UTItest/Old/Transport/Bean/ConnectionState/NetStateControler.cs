using System;
using System.Net;
using System.Threading.Tasks;

namespace UTI
{
    public class NetStateControler : INetStateControler
    {
        public INetClientState State { get; protected set; }

        public event Action<IPEndPoint> OnBeConnected;
        public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;
        public event Action<IPEndPoint> OnBeDisconnect;

        public NetStateControler(INetClientState state)
        {
            State = state;
        }

        public void __ChangeState__(INetClientState state)
        {
            State = state;
        }

        public Task<ConnectionResult> Connect(IPEndPoint ip)
        {
            return State.Connect(ip);
        }

        public Task<ConnectionResult> Disconnect(IPEndPoint ip)
        {
            return State.Disconnect(ip);
        }

        public Task<RunningResult> Dispose()
        {
            return State.Dispose();
        }

        public Task<ITransportResult> Send(IPEndPoint ip, string message, ITransportType type)
        {
            return State.Send(ip, message, type);
        }

        public Task<RunningResult> Start()
        {
            return State.Start();
        }
    }
    public class ConnectedConnection : INetClientState
    {

    }
    public class DisconnectedConnection : INetClientState
    {

    }

}
