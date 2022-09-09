using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UTI
{
    public class AsyncReliableTransport : ReliableTransportBasic
    {
        public const int TimeOutCount = 1500;
        public const int TimeOutMs = 10;

        public AsyncReliableTransport(Socket socket, int bufferSize) : base(socket, bufferSize)
        {
        }
        private ConcurrentDictionary<(IPEndPoint ip, int order), AsyncCheckingState> SendResult { get; } = new ConcurrentDictionary<(IPEndPoint, int), AsyncCheckingState>();


        public async Task<AsyncTransportResult> TrySendReliableDataAsync(IPEndPoint ip, IBytesFormater[] message)
        {
            //int hashcode = GetMessageHashCode();
            //this.SendReliableData_Async(hashcode, ip, message);
            var result = base.TrySendReliableData(ip, message,out var order);
            if (!result)
            {
                return AsyncTransportResult.Failed;
            }
            //SendResult[hashcode] = AsyncCheckingType.Sending;
            SendResult[(ip,order)] = AsyncCheckingState.Sending;
            int i = TimeOutCount;
            while (i > 0)
            {
                i--;
                await Task.Delay(TimeOutMs);
                if (SendResult.ContainsKey((ip, order)))
                {
                    if (SendResult[(ip, order)] == AsyncCheckingState.Successed)
                    {
                        SendResult.TryRemove((ip, order), out var d1);
                        return AsyncTransportResult.Successed;
                    }
                    else if (SendResult[(ip, order)] == AsyncCheckingState.Failed)
                    {
                        SendResult.TryRemove((ip, order), out var d2);
                        return AsyncTransportResult.Failed;
                    }
                }
            }
            SendResult.TryRemove((ip, order), out var d3);
            return AsyncTransportResult.TimeOut;
        }


        protected sealed override void OnReliableSendingFailed(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            if (SendResult.ContainsKey((ip,order)))
            {
                SendResult[(ip, order)] = AsyncCheckingState.Failed;
            }
            OnReliableSendingFailed_(order, ip, message);
            //var data = this.DeFormat(message);
            //if (data.Flag != null)
            //{
            //    if (data.Flag.IsData)
            //    {
            //        if (SendResult.ContainsKey(data.Flag.HashCode))
            //        {
            //            SendResult[data.Flag.HashCode] = AsyncCheckingType.Failed;
            //        }
            //    }
            //}
            //else
            //{
            //    OnReliableSendingFailed_(order, ip, message);
            //}
        }
        protected virtual void OnReliableSendingFailed_(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }


        protected sealed override void OnReliableSendingSuccessed(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            base.OnReliableSendingSuccessed(order, ip, message);
            if (SendResult.ContainsKey((ip, order)))
            {
                SendResult[(ip, order)] = AsyncCheckingState.Successed;
            }
            OnReliableSendingSuccessed_(order, ip,  message);
        }
        protected virtual void OnReliableSendingSuccessed_(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }


        #region OnSend
        //protected sealed override void OnSendReliableData(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{
        //    var data = this.DeFormat(message);
        //    if (data.Flag != null)
        //    {
        //        if (data.Flag.IsData)
        //        {
        //            this.OnSendAsyncReliableData(data.Flag.HashCode, ip, data.message);
        //        }
        //        else if (data.Flag.IsVerify)
        //        {
        //            this.OnSendReliableDataAsyncVerify(data.Flag.HashCode, ip, data.message);
        //        }
        //    }
        //    else
        //    {
        //        OnSendReliableData_(order, ip, message);
        //    }
        //}
        //protected virtual void OnSendAsyncReliableData(int hashcode, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        //private void OnSendReliableDataAsyncVerify(int hashcode, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        //protected virtual void OnSendReliableData_(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        #endregion
    }
}


