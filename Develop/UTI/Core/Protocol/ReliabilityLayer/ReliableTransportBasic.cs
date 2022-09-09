using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace UTI
{
    public class ReliableTransportBasic : TransportFormat
    {
        public const int FlashMs = 10;
        public const int SendingMs = 50;
        private ConcurrentDictionary<IPEndPoint, MessageClient> RepeatRecords { get; } = new ConcurrentDictionary<IPEndPoint, MessageClient>();

        private bool IsRepeating = false;
        private Task RepeatingTask = null;

        public ReliableTransportBasic(Socket socket, int bufferSize) : base(socket, bufferSize)
        {
        }

        public bool BeginReliableRepeat()
        {
            if (IsRepeating) return false;
            this.IsRepeating = true;
            this.RepeatingTask = Task.Run(OnRepeat);
            return true;
        }
        public bool StopReliableRepeat()
        {
            if (!IsRepeating) return false;
            this.IsRepeating = false;
            this.RepeatingTask.Dispose();
            this.RepeatingTask = null;
            this.RepeatRecords.Clear();
            return true;
        }
        private async void OnRepeat()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DateTime currentTime = DateTime.Now;

            while (this.IsRepeating)
            {
                await Task.Delay(FlashMs);
                currentTime = DateTime.Now;

                if (sw.ElapsedMilliseconds < SendingMs)
                {
                    continue;
                }

                foreach (var client in RepeatRecords.ToArray())
                {
                    if (sw.ElapsedMilliseconds >= SendingMs)
                    {
                        if (client.Value.IsTimeOut())
                        {
                            client.Value.ThrowAwayMesssage(OnReliableSendingFailed);
                        }
                        else
                        {
                            client.Value.SendRepeatMessage(currentTime, (ip, order, message) =>
                            {
                                SendReliableData(ip, order, message);
                                sw.Restart();
                            });
                        }
                    }
                }
            }
        }



        public bool TrySendReliableData(IPEndPoint ip, IBytesFormater[] message, out int order)
        {
            //是否放入推送列表
            MessageClient client = null;
            bool pushmessage = false;

            if (!RepeatRecords.ContainsKey(ip))
            {
                client = new MessageClient(ip, 0);
                if (RepeatRecords.TryAdd(ip, client))
                {
                    pushmessage = true;
                }
            }
            else
            {
                if (RepeatRecords.TryGetValue(ip, out client))
                {
                    pushmessage = true;
                }
            }
            if (client != null && pushmessage)
            {
                var messageContext = new MessageContext(message, base.BytesManager);
                order = client.PushMessage(messageContext);
                return true;
            }
            order = 0;
            return false;
        }
        private void SendReliableData(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            var data = Format(new ReliabilityFlag() { ReliableOrder = order, IsVerify = false }, message);
            base.SendFormat(ip, data);
        }
        private void SendVerification(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            var data = Format(new ReliabilityFlag() { ReliableOrder = order, IsVerify = true }, message);
            base.SendFormat(ip, data);
        }
        protected virtual void OnReliableSendingFailed(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }
        protected virtual void OnReliableSendingSuccessed(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }



        protected sealed override void OnReceivedFormat(IPEndPoint ip, IBytesFormater[] message)
        {
            var data = DeFormat(message);
            if (data.Flag != null)
            {
                if (data.Flag.IsVerify)
                {
                    ReceivedVerification(data.Flag.ReliableOrder, ip);
                }
                else if (data.Flag.IsData)
                {
                    ReceivedReliableData(data.Flag.ReliableOrder, ip, data.message);
                }
            }
            else
            {
                OnReceivedFormat_(ip, message);
            }
        }
        private void ReceivedReliableData(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            OnReceivedReliableData(order, ip, message);
            SendVerification(order, ip, message);
        }
        private void ReceivedVerification(int order, IPEndPoint ip)
        {
            if (RepeatRecords.TryGetValue(ip, out var client))
            {
                client.VerifyMessage(order, OnReliableSendingSuccessed);
            }
        }
        protected virtual void OnReceivedReliableData(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }
        protected virtual void OnReceivedFormat_(IPEndPoint ip, IBytesFormater[] message)
        {

        }




        private IBytesFormater[] Format(ReliabilityFlag Flag, IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is ReliabilityFlag)
            {
                var flag = message[0] as ReliabilityFlag;
                flag.ReliableOrder = Flag.ReliableOrder;
                flag.IsVerify = Flag.IsVerify;
                return message;
            }
            else
            {
                var flag = new ReliabilityFlag();
                flag.ReliableOrder = Flag.ReliableOrder;
                flag.IsVerify = Flag.IsVerify;

                var messageList = new List<IBytesFormater>() { flag };
                messageList.AddRange(message);
                return messageList.ToArray();
            }
        }
        private (ReliabilityFlag Flag, IBytesFormater[] message) DeFormat(IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is ReliabilityFlag)
            {
                var flag = message[0] as ReliabilityFlag;
                return (flag, message.Skip(1).ToArray());
            }
            else
            {
                return (null, message);
            }
        }





        #region OnSend
        //protected sealed override void OnSendFormat(int sendedFormatLength, IPEndPoint ip, ITransportBytesFormat[] message)
        //{
        //    var data = DeFormat(message);
        //    if (data.Flag != null)
        //    {
        //        if (data.Flag.IsVerify)
        //        {
        //            OnSendVerification(data.Flag.ReliableOrder, ip, data.message);
        //        }
        //        else if (data.Flag.IsData)
        //        {
        //            OnSendReliableData(data.Flag.ReliableOrder, ip, data.message);
        //        }
        //    }
        //    else
        //    {
        //        OnSendFormat_Raw(sendedFormatLength, ip, message);
        //    }
        //}
        //protected virtual void OnSendReliableData(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        //private void OnSendVerification(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        //protected virtual void OnSendFormat_Raw(int sendedFormatLength, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        #endregion
    }
}


