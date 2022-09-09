using System;

namespace UTI
{
    /// <summary>
    /// 基础数据报协议     
    /// 包含应有的基本功能
    /// </summary>
    public interface IBasicDatagramProtocol
    {
        /// <summary>
        /// 接受到信息
        /// </summary>
        event Action<string, TransportDatagramType, object[]> OnReceive;
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="targetIp"></param>
        /// <param name="sendType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TransportResultType Send(string targetIp, TransportDatagramType sendType, object[] args);
    }

}