using System;
using System.Collections.Generic;

namespace UTI
{
    /// <summary>
    /// 数据报协议: 提供基础的 OnReceive 和 Send 代理
    /// 使用传入的 Transporter 作为传输底层
    /// 使用传入的 Converter 作为协议转换器
    /// 使用传入的 Proxy 进行对自身二次代理, 
    /// 如果需要使用二次细分代理, 请使用此Proxy对象的方法
    /// </summary>
    public class UniversalDatagramProtocol : IBasicDatagramProtocol
    {
        /// <summary>
        /// 传输器
        /// </summary>
        private ITransportBasic Transporter;
        /// <summary>
        /// 转换器
        /// </summary>
        private IDatagramsConverter Converter;

        /// <summary>
        /// 构造通用数据报协议代理
        /// 使用JSON结构数据报转换器
        /// 不使用多级代理
        /// </summary>
        /// <param name="transport">传输层</param>
        public UniversalDatagramProtocol(ITransportBasic transport) : this(transport, new JsonDatagramConverter()) { }
        /// <summary>
        /// 构造通用数据报协议代理
        /// </summary>
        /// <param name="transport">传输层</param>
        /// <param name="converter">数据报转换器</param>
        /// <param name="proxy">二次代理</param>
        public UniversalDatagramProtocol(ITransportBasic transport, IDatagramsConverter converter)
        {
            Transporter = transport;
            Converter = converter;
            StartProxy();
        }

        ~UniversalDatagramProtocol()
        {
            StopProxy();
        }
        /// <summary>
        /// 默认在实例化对象后进行代理
        /// </summary>
        public void StartProxy()
        {
            if (Transporter != null)
                Transporter.OnReceivedRawBytes += OnTransportReceive;
            else
            {
                Log.Error("UniversalDatagramProtocol.Transporter is Null !");
            }
        }
        /// <summary>
        /// 默认在析构函数后进行停止代理
        /// </summary>
        public void StopProxy()
        {
            if (Transporter != null)
                Transporter.OnReceivedRawBytes -= OnTransportReceive;
            else
            {
                Log.Error("UniversalDatagramProtocol.Transporter is Null !");
            }
        }



        private void OnTransportReceive(string userip, byte type, string message)
        {
            try
            {
                object[] msg = Converter.StringToObjects(message);
                OnReceive?.Invoke(userip, (TransportDatagramType)type, msg);
            }
            catch (Exception ex)
            {
                Log.Error("UniversalDatagramProtocol.OnTransportReceive", ex);
                throw;
            }
            catch
            {
                Log.Error("UniversalDatagramProtocol.OnTransportReceive Error !");
                throw;
            }
        }




        public event Action<string, TransportDatagramType, object[]> OnReceive;
        public TransportResultType Send(string targetIp, TransportDatagramType sendType, object[] args)
        {
            if (Transporter == null)
            {
                Log.Error("UniversalDatagramProtocol.Transporter is Null !");
                return TransportResultType.Missed;
            }
            try
            {
                string msg = Converter.ObjectsToString(args);
                return (TransportResultType)this.Transporter.Send(targetIp, (byte)sendType, msg);
            }
            catch (Exception ex)
            {
                Log.Error("UniversalDatagramProtocol.Send", ex);
                throw;
            }
            catch
            {
                Log.Error("UniversalDatagramProtocol.SendMessage Error !");
                throw;
            }
        }
    }

}

