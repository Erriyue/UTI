namespace UTI
{

    /// <summary>
    /// 数据报协议代理器    
    /// 对数据报协议进行代理
    /// </summary>
    public interface IDatagramProtocolProxy : IBasicDatagramProtocol
    {
        /// <summary>
        /// 开始代理
        /// </summary>
        /// <param name="proxy"></param>
        void StartProxy(IBasicDatagramProtocol proxy);
        /// <summary>
        /// 结束代理
        /// </summary>
        /// <param name="proxy"></param>
        void StopProxy(IBasicDatagramProtocol proxy);
    }

}