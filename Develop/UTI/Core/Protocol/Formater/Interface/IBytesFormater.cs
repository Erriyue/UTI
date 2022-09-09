namespace UTI
{
    /// <summary>
    /// 传输字节格式化器  必须保留无参构造函数!
    /// </summary>
    public interface IBytesFormater
    {
        /// <summary>
        /// 传输字节类型 TransportBytesType
        /// </summary>
        byte Type { get; }
        /// <summary>
        /// 是否限制最大长度
        /// </summary>
        int? MaxLength { get; }
        /// <summary>
        /// 生成字节
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();
        /// <summary>
        /// 反序列化字节
        /// </summary>
        /// <param name="bytes">提供可读取的字节</param>
        /// <param name="length">实际使用的字节的长度</param>
        void SetBytes(byte[] bytes, out int length);
    }
}


