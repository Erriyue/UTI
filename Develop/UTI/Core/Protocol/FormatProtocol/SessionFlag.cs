using System;
using System.Collections.Generic;

namespace UTI
{

    public class SessionFlag : IBytesFormater
    {
        public byte Type => (byte)BytesType.SessionFlag;
        public int? MaxLength => 1;
        public SessionType DataType { get; set; }

        public byte[] GetBytes()
        {
            List<byte> list = new List<byte>();
            list.Add((byte)DataType);
            return list.ToArray();
        }

        public void SetBytes(byte[] bytes, out int length)
        {
            this.DataType = (SessionType)bytes[0];
            length = 1;
        }
    }
}


