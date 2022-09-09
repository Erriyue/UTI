using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace UTI
{
    /// <summary>
    /// 通用传输接口
    /// </summary>
    public class UniversalTransport : SessionInterface
    {
        public IObjectConverter DataConverter { get; }
        public UniversalTransport(Socket socket, IObjectConverter converter) : base(socket, ushort.MaxValue - 1024)
        {
            this.DataConverter = converter;
        }



        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip">目标ip</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息内容</param>
        /// <param name="objects">携带字节格式对象</param>
        public void SendMessage(IPEndPoint ip, byte mark, string message, IBytesFormater[] objects)
        {
            var data = Format(new UniversalTransportFlag() { Mark = mark, Message = message }, objects);
            base.TrySendReliableData(ip, data, out var order);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip">目标ip</param>
        /// <param name="mark">标记</param>
        /// <param name="message">消息内容</param>
        /// <param name="objects">携带Json格式对象</param>
        public void SendMessage(IPEndPoint ip, byte mark, string message, object[] objects)
        {
            var data = Format(new UniversalTransportFlag() { Mark = mark, Message = message },
                new IBytesFormater[] { new UniversalTransportArgs() { Message = this.DataConverter.ObjectsToString(objects) } });
            base.TrySendReliableData(ip, data, out var order);
        }




        protected sealed override void OnReceivedReliableData(int order, IPEndPoint ip, IBytesFormater[] message)
        {
            var data = DeFormat(message);
            if (data.Flag != null)
            {
                if (data.message.Length == 1 && data.message[0] is UniversalTransportArgs)
                {
                    OnReceiveMessage(ip, data.Flag.Mark, data.Flag.Message, (this.DataConverter.StringToObjects((data.message[0] as UniversalTransportArgs).Message)));
                }
                else
                {
                    OnReceiveMessage(ip, data.Flag.Mark, data.Flag.Message, data.message);
                }
            }
            else
            {
                OnReceivedReliableData_(order, ip, message);
            }
        }
        protected virtual void OnReceiveMessage(IPEndPoint ip, byte mark, string message, IBytesFormater[] objects)
        {

        }
        protected virtual void OnReceiveMessage(IPEndPoint ip, byte mark, string message, object[] objects)
        {

        }
        protected virtual void OnReceivedReliableData_(int order, IPEndPoint ip, IBytesFormater[] message)
        {

        }




        private IBytesFormater[] Format(UniversalTransportFlag Flag, IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is UniversalTransportFlag)
            {
                var flag = message[0] as UniversalTransportFlag;
                flag.Mark = Flag.Mark;
                flag.Message = Flag.Message;

                return message;
            }
            else
            {
                var flag = new UniversalTransportFlag();
                flag.Mark = Flag.Mark;
                flag.Message = Flag.Message;

                var messageList = new List<IBytesFormater>() { flag };
                messageList.AddRange(message);
                return messageList.ToArray();
            }
        }
        private (UniversalTransportFlag Flag, IBytesFormater[] message) DeFormat(IBytesFormater[] message)
        {
            if (message.Length >= 1 && message[0] is UniversalTransportFlag)
            {
                var flag = message[0] as UniversalTransportFlag;
                return (flag, message.Skip(1).ToArray());
            }
            else
            {
                return (null, message);
            }
        }




        #region OnSendError
        //protected sealed override void OnReliableSendingFailed_(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{
        //    var data = DeFormat(message);
        //    if (data.Flag != null)
        //    {
        //        OnMessageSendingFaild(ip, data.Flag.Mark, data.Flag.Message, data.message);
        //    }
        //    else
        //    {
        //        OnReliableSendingFailed__(order, ip, message);
        //    }
        //}
        //protected virtual void OnMessageSendingFaild(IPEndPoint ip, byte mark, string message, ITransportBytesFormat[] objects)
        //{

        //}
        //protected virtual void OnReliableSendingFailed__(int order, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        #endregion
    }
}


