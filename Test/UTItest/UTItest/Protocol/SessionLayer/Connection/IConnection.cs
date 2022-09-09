using System.Net;

namespace UTI
{
    public interface IConnection
    {
        IPEndPoint IP { get; }
        bool IsServer { get; }
        int Ping { get; }
        ConnectionState State { get; }
    }
}