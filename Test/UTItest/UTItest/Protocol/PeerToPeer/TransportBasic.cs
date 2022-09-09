using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections;

namespace UTI
{

    /// <summary>
    /// 端到端字节传输基础
    /// 能够提供端到端字节传输的封装
    /// </summary>
    public abstract class TransportBasic
    {
        protected int BufferSize;
        protected byte[] Buffer;
        protected bool IsListening;
        protected Task ListeningTask;
        private IPEndPoint IP;
        private Socket Socket;
        public TransportBasic(Socket socket, int bufferSize)
        {
            this.BufferSize = bufferSize;
            Buffer = new byte[BufferSize];
            Socket = socket;
        }


        public bool BeginListening(IPEndPoint iP)
        {
            if (this.IsListening) return false;
            IP = iP;
            this.IsListening = true;
            this.ListeningTask = Task.Run(OnListening);
            return true;
        }
        public bool StopListening()
        {
            if (!this.IsListening) return false;
            this.IsListening = false;
            this.ListeningTask.Dispose();
            this.ListeningTask = null;
            return true;
        }
        private async void OnListening()
        {
            while (this.IsListening)
            {
                try
                {
                    var result = await Socket.ReceiveMessageFromAsync(Buffer, SocketFlags.None, IP);
                    ReceivedBytes((IPEndPoint)result.RemoteEndPoint, Buffer.Take(result.ReceivedBytes).ToArray());
                }
                catch (Exception ex)
                {
                    Log.Error("TransportBasic: ", ex);
                }
                catch
                {
                    Log.Error("TransportBasic Error !");
                }
            }
        }



        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="ip">目标ip</param>
        /// <param name="message">字节</param>
        private void ReceivedBytes(IPEndPoint ip, byte[] message)
        {
            OnReceivedBytes(ip, message);
        }
        protected virtual void OnReceivedBytes(IPEndPoint ip, byte[] message)
        {

        }
        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="ip">目标ip</param>
        /// <param name="message">字节</param>
        /// <returns>返回成功发送的字节数</returns>
        protected void SendBytes(IPEndPoint ip, byte[] message)
        {
            if (message.Length >= this.BufferSize)
            {
                throw new Exception("Message bytes[] Array Overflow !");
            }
            var result = Socket.SendTo(message, ip);
            if (result == message.Length)
            {
                OnSendBytes(result, ip, message);
            }
        }
        protected virtual void OnSendBytes(int sendBytesLength, IPEndPoint ip, byte[] message)
        {

        }
    }


    //public class RUDPTransportProtocol : RUDPTransportFormat
    //{
    //    public RUDPTransportProtocol(IPEndPoint iP, Socket socket) : base(iP, socket)
    //    {
    //        base.OnRUDPReceivedMessage += OnBaseReceived;
    //    }

    //    private void OnBaseReceived(IPEndPoint ip, string message, int order, byte type)
    //    {
    //        this.OnReceivedMessage?.Invoke(ip, message, new RUDPTransportType(type, order));
    //    }

    //    public event Action<IPEndPoint, string, ITransportType> OnReceivedMessage;

    //    public ITransportResult Send(IPEndPoint ip, string message, ITransportType type)
    //    {
    //        var result = base.Send(ip, message, (type as RUDPTransportType).Order, type.Type);
    //        return new TransportResult(result > 0 ? TransportResultType.Successed : TransportResultType.Missed, result.ToString());
    //    }
    //}
}
