using System;
using System.Collections.Generic;
using System.Text;

namespace UTI
{
    public class ProtocolPackage
    {
        public int ID;

        public string IP;
        public TransportDatagramType Type;
        public string Message;

        public bool isReceived = false;

        public ProtocolPackage(int ID,string _ip, TransportDatagramType _type,string _message)
        {
            this.ID = ID;
            IP = _ip;
            Type = _type;
            Message = _message;
            //新实例化出来的必定是未收到状态
            isReceived = false;
        }
    }
}
