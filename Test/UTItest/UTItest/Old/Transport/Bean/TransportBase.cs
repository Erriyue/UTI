using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings;

namespace UTI
{
    //public interface INetworkNode
    //{
    //    IPEndPoint IP { get; }
    //}
    //public interface ITransportNode : INetworkNode
    //{
    //    int Order { get; }
    //    int BufferSize { get; }
    //}
    //public interface IVirtualClient : ITransportNode
    //{
    //    void Push(TransportVirtualPackage a);
    //    TransportVirtualPackage GetFirst();
    //    bool IsTimeOut();
    //}
    //public interface IClient : IVirtualClient
    //{
    //    List<IVirtualClient> Connections { get; }
    //}




    public class TransportVirtualPackage
    {
        public string Message;
        public RUDPTransportType Type;

        public TransportVirtualPackage(int orderID, string message, RUDPTransportType type)
        {
            Message = message;
            Type = type;
        }
    }
    public class RUDPClient /*: IVirtualClient*/
    {
        public int Order { get; protected set; }
        public int BufferSize { get; }
        public IPEndPoint IP { get; }
        public List<TransportVirtualPackage> SendList { get; } = new List<TransportVirtualPackage>();


        public RUDPClient(IPEndPoint iP)
        {
            BufferSize = TransportSetting.BufferSize;
            IP = iP;
            Order = 0;
        }

        public void Push(TransportVirtualPackage a)
        {
            SendList.Add(a);
        }

        public TransportVirtualPackage GetFirst()
        {
            if (SendList.Count <= 0) return null;
            else return SendList[0];
        }

        public void RemoveOrder(int order)
        {
            SendList.RemoveAll(e => e.Type.Order == order);
            Order++;
        }

        public bool IsTimeOut()
        {
            return false;
        }
    }
}
