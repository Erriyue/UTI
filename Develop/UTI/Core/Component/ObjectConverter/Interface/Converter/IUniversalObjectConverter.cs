
namespace UTI
{
    /// <summary>
    /// 通用协议数据类型转换器
    /// </summary>
    public interface IUniversalObjectConverter
    {
        /// <summary>
        /// 把数据转换为字符串
        /// </summary>
        /// <returns></returns>
        (string mark, string str) ConverToString(System.Type type, object obj);
        /// <summary>
        /// 把字符串转换为数据
        /// </summary>
        /// <returns></returns>
        (System.Type type, object arg) ConverToObject(string typemark, string str);
    }

}

