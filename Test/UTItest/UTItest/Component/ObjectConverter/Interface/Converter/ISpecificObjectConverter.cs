using System;

namespace UTI
{

    /// <summary>
    /// 特定协议数据类型转换器
    /// </summary>
    public interface ISpecificObjectConverter
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        Type DataType { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        string DataMark { get; }
        /// <summary>
        /// 把数据转换为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string ObjecttoJson(object obj);
        /// <summary>
        /// 把字符串转换为数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        object JsontoObject(string str);
    }

}
