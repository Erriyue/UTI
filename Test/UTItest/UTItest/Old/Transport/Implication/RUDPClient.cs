using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UTI
{
    public class RUDPClient2 : Client
    {
        public SortedDictionary<int, byte[]> m_SendMessageQueue = new SortedDictionary<int, byte[]>();
        public ProtocolPackage LastedReceivedMessage;

        private RUDPTransport m_Transport;
        public RUDPClient2(IPEndPoint _ip, RUDPTransport _transport) : base(_ip)
        {
            m_Transport = _transport;
        }

        public void UpdateLastedMessage(int MessageOrder, TransportDatagramType Type,string Message)
        {
            if(LastedReceivedMessage == null || LastedReceivedMessage.isReceived)
            {
                LastedReceivedMessage = new ProtocolPackage(MessageOrder, IPEP.ToString(), Type, Message);
            }
            if (LastedReceivedMessage.ID > MessageOrder)
            {
                Log.Error("过时消息,已放弃");
                return;
            }
            else if (LastedReceivedMessage.ID == MessageOrder && !LastedReceivedMessage.isReceived)
            {
                //收到序号和自身一致的消息,进行检查,如果消息一致,那么就向协议层应用这条消息,并向客户端发送收到请求
                //如果不一致,那么就更新这条消息,不做任何操作
                if (Message == LastedReceivedMessage.Message)
                {
                    LastedReceivedMessage.isReceived = true;

                    m_Transport.ComplyMessage(this, LastedReceivedMessage);
                    UpdateTime();

                    Log.Error("消息已被客户端确认");
                }
                else
                {
                    LastedReceivedMessage = new ProtocolPackage(MessageOrder, IPEP.ToString(), Type, Message);
                    Log.Error("消息不一致");
                }
            }
            else if(LastedReceivedMessage.ID == MessageOrder + 1)
            {
                //收到客户端的新消息,进行应用
                if (LastedReceivedMessage.isReceived == true)
                {
                    LastedReceivedMessage = new ProtocolPackage(MessageOrder, IPEP.ToString(), Type, Message);
                    Log.Error("收到最新消息");
                }
                else
                {
                    Log.Error("上一个消息还未被确认,此消息已被丢弃");
                }
            }
            else if(MessageOrder > LastedReceivedMessage.ID + 1)
            {
                //收到后面才会发送的消息,通常来说这是不正常的,客户端那边只有前面的消息发送并被接收到才会发后面的
                //可能是客户端没有严格遵循规定,前面后面的消息一块发送了
                //直接丢弃即可
                Log.Error("错误!收到过于靠后的消息,客户端或程序极有可能出错,建议断开连接");
            }
        }

        private int LastedSendOrderID = 0;
        public int GetCurrentOrder() => LastedSendOrderID;
        public KeyValuePair<int, byte[]>? FirstMessage()
        {
            if (m_SendMessageQueue.Count == 0)
                return null;
            else
                return m_SendMessageQueue.First();
        }
        public void PushQueue(byte[] rawMessage)
        {
            if (!m_Transport.canQueue && m_SendMessageQueue.Count != 0)
                return;
            Log.Server(LastedSendOrderID.ToString());
            m_SendMessageQueue.Add(LastedSendOrderID, rawMessage);
            LastedSendOrderID++;
        }
        public void AcknowledgeMessage(int orderID)
        {
            if (m_SendMessageQueue.ContainsKey(orderID))
            {
                m_SendMessageQueue.Remove(orderID);
            }
            else
            {
                //Log.Error($"{IP}:错误!消息发送队列不存在ID为{orderID}的消息");
            }
        }

    }
}
