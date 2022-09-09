using System.Net;
using System.Collections.Concurrent;
using System;

namespace UTI
{
    public class MessageClient
    {
        public const int RepeatTime = 500;
        public const int MaxRepeatCount = 20;
        private ConcurrentDictionary<int, MessageContext> messageBuffer { get; } = new ConcurrentDictionary<int, MessageContext>();
        public IPEndPoint Ip { get; }
        public int CurrentSendedOrder { get; private set; }
        public int CurrentVerifyOrder { get; private set; }

        public MessageClient(IPEndPoint ip, int currentOrder)
        {
            this.Ip = ip;
            this.CurrentVerifyOrder = currentOrder;
            this.CurrentSendedOrder = currentOrder;
        }


        /// <summary>
        /// 放入一条消息
        /// </summary>
        /// <param name="order"></param>
        /// <param name="context"></param>
        public int PushMessage(MessageContext context)
        {
            var order = this.CurrentSendedOrder;
            messageBuffer[order] = context;
            var item = messageBuffer[order];
            item.Init();
            this.CurrentSendedOrder++;
            return order;
        }
        /// <summary>
        /// 尝试发送数据
        /// </summary>
        /// <param name="order"></param>
        /// <param name="OnSended"></param>
        public void SendRepeatMessage(DateTime currentTime, Action<int, IPEndPoint, IBytesFormater[]> OnSended)
        {
            //寻找当前order进行发送
            if (messageBuffer.ContainsKey(CurrentVerifyOrder))
            {
                var message = messageBuffer[CurrentVerifyOrder];
                var order = CurrentVerifyOrder;
                if (message.SendTime == default || (currentTime - message.SendTime).TotalMilliseconds >= RepeatTime)
                {
                    OnSended?.Invoke(order, Ip, message.Message);
                    message.Sended(currentTime);
                }
            }
        }
        /// <summary>
        /// 确认一条消息
        /// </summary>
        /// <param name="order"></param>
        public void VerifyMessage(int order, Action<int, IPEndPoint, IBytesFormater[]> OnVerified)
        {
            if (order == CurrentVerifyOrder && order < CurrentSendedOrder)
            {
                if (messageBuffer.ContainsKey(CurrentVerifyOrder))
                {
                    if (messageBuffer.TryRemove(CurrentVerifyOrder, out var message))
                    {
                        this.CurrentVerifyOrder++;
                        OnVerified?.Invoke(order, this.Ip, message.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 丢弃消息
        /// </summary>
        /// <param name="order"></param>
        public void ThrowAwayMesssage(Action<int, IPEndPoint, IBytesFormater[]> OnThrow)
        {
            if (messageBuffer.ContainsKey(CurrentVerifyOrder))
            {
                if (messageBuffer.TryRemove(CurrentVerifyOrder, out var message))
                {
                    OnThrow?.Invoke(CurrentVerifyOrder, Ip, message.Message);
                    this.CurrentVerifyOrder++;
                }
            }
        }
        /// <summary>
        /// 消息超时
        /// </summary>
        /// <returns></returns>
        public bool IsTimeOut()
        {
            if (CurrentSendedOrder > CurrentVerifyOrder)
            {
                if (messageBuffer.ContainsKey(CurrentVerifyOrder))
                {
                    var msg = messageBuffer[CurrentVerifyOrder];
                    if (msg == null || msg.SendCount >= MaxRepeatCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}


