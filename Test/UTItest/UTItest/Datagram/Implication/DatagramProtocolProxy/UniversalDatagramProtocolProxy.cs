using System;

namespace UTI
{
    /// <summary>
    /// 通用数据报协议代理
    /// 对基础数据报协议的相关功能进行代理
    /// </summary>
    public class UniversalDatagramProtocolProxy : IDatagramProtocolProxy
    {
        public UniversalDatagramProtocolProxy()
        {
        }

        public UniversalDatagramProtocolProxy(string proxyName)
        {
            Name = proxyName;
        }

        public string Name { get; private set; }
        protected IBasicDatagramProtocol Raw { get; private set; }



        public void StartProxy(IBasicDatagramProtocol proxy)
        {
            if (Raw != null) return;
            if (proxy == null) return;
            proxy.OnReceive += OnReceived;

            this.Raw = proxy;
            OnProxyStart?.Invoke(proxy);
        }
        public void StopProxy(IBasicDatagramProtocol proxy)
        {
            if (Raw == null) return;
            if (proxy == null) return;
            proxy.OnReceive -= OnReceived;

            this.Raw = null;
            OnProxyEnd?.Invoke(proxy);
        }



        public virtual TransportResultType Send(string targetIp, TransportDatagramType sendType, object[] args)
        {
            if (this.Raw != null)
            {
                this.OnSending?.Invoke(targetIp, sendType, args);
                var result = this.Raw.Send(targetIp, sendType, args);
                this.OnSended?.Invoke(targetIp, sendType, args, result);
                return result;
            }
            else return TransportResultType.Missed;
        }
        protected virtual void OnReceived(string ip, TransportDatagramType type, object[] args)
        {
            OnReceive?.Invoke(ip, type, args);
        }



        /// <summary>
        /// 开始代理事件
        /// </summary>
        public event Action<IBasicDatagramProtocol> OnProxyStart;
        /// <summary>
        /// 结束代理事件
        /// </summary>
        public event Action<IBasicDatagramProtocol> OnProxyEnd;
        /// <summary>
        /// 发送消息时事件
        /// </summary>
        public event Action<string, TransportDatagramType, object[]> OnSending;
        /// <summary>
        /// 发送消息事件
        /// </summary>
        public event Action<string, TransportDatagramType, object[], TransportResultType> OnSended;
        /// <summary>
        /// 接受消息事件
        /// </summary>
        public event Action<string, TransportDatagramType, object[]> OnReceive;
    }

}

