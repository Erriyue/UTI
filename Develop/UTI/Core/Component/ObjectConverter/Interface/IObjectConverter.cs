
namespace UTI
{
    /// <summary>
    /// 协议数据转换器
    /// </summary>
    public interface IObjectConverter
    {
        /// <summary>
        /// 把参数转换为Message
        /// 参数如果是类类型,请为类保留一个无参构造函数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        string ObjectsToString(object[] args);
        /// <summary>
        /// 把Message转换为参数
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        object[] StringToObjects(string message);
    }
}
