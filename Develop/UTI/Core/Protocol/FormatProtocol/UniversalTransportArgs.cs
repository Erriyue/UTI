using System;
using System.Collections.Generic;

namespace UTI
{
    public class UniversalTransportArgs : IBytesFormater
    {
        public byte Type => (byte)BytesType.UniversalTransportArgs;
        public int? MaxLength => null;
        public string Message { get; set; }
        public byte[] GetBytes()
        {
            byte[] msg;
            if (this.Message == null)
            {
                msg = new byte[0];
            }
            else
            {
                msg = System.Text.Encoding.UTF8.GetBytes(Message);
            }
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes((int)msg.Length));
            if (msg.Length > 0)
            {
                list.AddRange(msg);
            }
            return list.ToArray();
        }
        public void SetBytes(byte[] bytes, out int length)
        {
            var l = BitConverter.ToInt32(bytes, 0);
            if (l <= 0)
            {
                this.Message = null;
                length = 4;
                return;
            }
            else
            {
                this.Message = System.Text.Encoding.UTF8.GetString(bytes, 4, l);
                length = 4 + l;
                return;
            }
        }
    }
}


