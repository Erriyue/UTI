using System;
using System.Linq;

namespace UTI
{
    public class ReliabilityFlag : IBytesFormater
    {
        public byte Type => (byte)BytesType.ReliabilityFlag;
        public int? MaxLength => 5;
        public int ReliableOrder { get; set; }
        public bool IsVerify { get; set; }
        public bool IsData => !IsVerify;
        public byte[] GetBytes()
        {
            var byteInts = BitConverter.GetBytes(this.ReliableOrder);
            byte[] isverify = new byte[] { (IsVerify ? (byte)1 : (byte)0) };
            return byteInts.Concat(isverify).ToArray();
        }
        public void SetBytes(byte[] bytes,out int length)
        {
            this.ReliableOrder = BitConverter.ToInt32(bytes, 0);
            this.IsVerify = bytes[4] == (byte)1;
            length = 5;
        }
    }
}


