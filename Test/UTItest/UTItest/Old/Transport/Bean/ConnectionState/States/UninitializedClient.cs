using System;
using System.Net;
using System.Threading.Tasks;

namespace UTI
{
    public class UninitializedClient : INetClientState
    {
        public ITransportProtocol Transport { get; }
        public INetStateControler Controler { get; }

        public UninitializedClient(ITransportProtocol transport, INetStateControler controler)
        {
            Transport = transport;
            Controler = controler;
        }

        public Task<RunningResult> Start()
        {
            Controler.__ChangeState__(new NormalClient(this.Transport, this.Controler));
            return Task.FromResult(new RunningResult(true, "Successed"));
        }





        public Task<ConnectionResult> Connect(IPEndPoint ip)
        {
            return Task.FromResult(new ConnectionResult(false, "UninitializedClient"));
        }
        public Task<RunningResult> Dispose()
        {
            return Task.FromResult(new RunningResult(false, "UninitializedClient"));
        }
        public Task<ConnectionResult> Disconnect(IPEndPoint ip)
        {
            return Task.FromResult(new ConnectionResult(false, "UninitializedClient"));
        }
        public Task<ITransportResult> Send(IPEndPoint ip, string message, ITransportType type)
        {
            return Task.FromResult(new TransportResult(TransportResultType.Failed, "UninitializedClient") as ITransportResult);
        }
        public event Action<IPEndPoint> OnBeConnected;
        public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;
        public event Action<IPEndPoint> OnBeDisconnect;
    }

    public class NormalClient : INetClientState
    {
        public ITransportProtocol Transport { get; }
        public INetStateControler Controler { get; }

        public NormalClient(ITransportProtocol transport, INetStateControler controler)
        {
            Transport = transport;
            Controler = controler;

            Transport.on
        }

        public Task<ConnectionResult> Connect(IPEndPoint ip)
        {
            return Task.FromResult(new ConnectionResult(false, "NormalClient"));
        }
        public event Action<IPEndPoint> OnBeConnected;
        public Task<ITransportResult> Send(IPEndPoint ip, string message, ITransportType type)
        {
            return Task.FromResult(new TransportResult(TransportResultType.Failed, "NormalClient") as ITransportResult);
        }
        public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;
        public Task<ConnectionResult> Disconnect(IPEndPoint ip)
        {
            return Task.FromResult(new ConnectionResult(false, "NormalClient"));
        }
        public event Action<IPEndPoint> OnBeDisconnect;
        public Task<RunningResult> Dispose()
        {
            return Task.FromResult(new RunningResult(false, "NormalClient"));
        }







        public Task<RunningResult> Start()
        {
            return Task.FromResult(new RunningResult(false, "NormalClient"));
        }
    }
}
