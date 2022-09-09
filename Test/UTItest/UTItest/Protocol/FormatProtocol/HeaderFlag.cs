using System;
using System.Collections.Generic;

namespace UTI
{
    public class HeaderFlag : IBytesFormater
    {
        public byte Type => byte.MinValue;
        public int? MaxLength => byte.MaxValue;
        private IBytesFormatManager Manager { get; }
        public IBytesFormater[] Formats { get; set; }
        public HeaderFlag()
        {
            Manager = new BytesFormaterManager();
        }
        public HeaderFlag(IBytesFormatManager manager)
        {
            Manager = manager;
        }

        public byte[] GetBytes()
        {
            List<byte> byteArray = new List<byte>() { (byte)Formats.Length };
            for (int i = 0; i < Formats.Length; i++)
            {
                byteArray.Add(Formats[i].Type);
            }
            if (byteArray.Count > this.MaxLength.Value)
            {
                throw new Exception("Formats Nums Overflow !");
            }
            return byteArray.ToArray();
        }
        public void SetBytes(byte[] bytes, out int totallength)
        {
            var length = bytes[0];
            List<IBytesFormater> formats = new List<IBytesFormater>();
            for (int i = 1; i < length + 1; i++)
            {
                formats.Add(Manager.GetFormatDefined(bytes[i]));
            }
            this.Formats = formats.ToArray();
            totallength = length + 1;
        }

        public IBytesFormater[] GetInstances()
        {
            return this.Formats;
        }
        public void SetInstances(IBytesFormater[] formats)
        {
            if (formats.Length >= byte.MaxValue)
            {
                throw new Exception("Transports Formats Too Much !");
            }
            this.Formats = formats;
        }
    }
}


