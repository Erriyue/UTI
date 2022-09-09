using System;
using System.Collections.Generic;

namespace UTI
{
    public class UniversalTransportFlag : IBytesFormater
    {
        public byte Type => (byte)BytesType.UniversalTransportFlag;
        public int? MaxLength => null;

        public byte Mark { get; set; }
        public string Message { get; set; }
        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(Mark);
            if (Message == null)
            {
                bytes.AddRange(BitConverter.GetBytes((int)0));
            }
            else
            {
                var msg = System.Text.Encoding.UTF8.GetBytes(Message);
                bytes.AddRange(BitConverter.GetBytes(msg.Length));
                bytes.AddRange(msg);
            }
            return bytes.ToArray();
        }
        public void SetBytes(byte[] bytes, out int length)
        {
            this.Mark = bytes[0];
            this.Message = null;
            int msgLength = BitConverter.ToInt32(bytes, 1);
            if (msgLength <= 0)
            {
                length = 5;
                this.Message = null;
                return;
            }
            else
            {
                this.Message = System.Text.Encoding.UTF8.GetString(bytes, 5, msgLength);
                length = 5 + msgLength;
                return;
            }
        }
    }
}


