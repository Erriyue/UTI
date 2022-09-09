using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace UTI
{
    /// <summary>
    /// 格式化传输
    /// 可以拓展性的传输符合字节传输接口的参数
    /// </summary>
    public class TransportFormat : TransportBasic
    {
        public TransportFormat(Socket socket, int bufferSize) : base(socket, bufferSize)
        {
        }

        public BytesFormaterManager BytesManager { get; set; } = new BytesFormaterManager();



        public void SendFormat(IPEndPoint ip, IBytesFormater[] message)
        {
            var data = Format(message);
            base.SendBytes(ip, data);
        }



        protected sealed override void OnReceivedBytes(IPEndPoint ip, byte[] message)
        {
            var data = DeFormat(message);
            OnReceivedFormat(ip, data);
        }
        protected virtual void OnReceivedFormat(IPEndPoint ip, IBytesFormater[] message)
        {

        }



        private byte[] Format(IBytesFormater[] message)
        {
            var typesdefined = new HeaderFlag(this.BytesManager);
            typesdefined.SetInstances(message);

            byte[] byteArray = typesdefined.GetBytes();
            for (int i = 0; i < message.Length; i++)
            {
                byteArray = byteArray.Concat(message[i].GetBytes()).ToArray();
            }
            return byteArray;
        }
        private IBytesFormater[] DeFormat(byte[] message)
        {
            int startIndex = 0;

            //读取首部
            var header = new HeaderFlag(this.BytesManager);
            Take(header, message, startIndex, message.Length - startIndex, out var realLength);
            startIndex += realLength;

            List<IBytesFormater> formats = new List<IBytesFormater>();
            formats.AddRange(header.GetInstances());

            for (int i = 0; i < formats.Count; i++)
            {
                int maxLength = message.Length - startIndex;
                if (formats[i].MaxLength != null && formats[i].MaxLength < maxLength)
                {
                    maxLength = formats[i].MaxLength.Value;
                }
                Take(formats[i], message, startIndex, maxLength, out var rl);
                startIndex += rl;
            }
            var byteArray = formats.ToArray();
            return byteArray;
        }
        private void Take(IBytesFormater value, byte[] message, int startindex, int readlength, out int length)
        {
            value.SetBytes(message.Skip(startindex).Take(readlength).ToArray(), out length);
        }




        #region OnSend
        //protected sealed override void OnSendBytes(int sendBytesLength, IPEndPoint ip, byte[] message)
        //{
        //    var data = DeFormat(message);
        //    OnSendFormat(sendBytesLength, ip, data);
        //}
        //protected virtual void OnSendFormat(int sendedFormatLength, IPEndPoint ip, ITransportBytesFormat[] message)
        //{

        //}
        #endregion
    }
}
