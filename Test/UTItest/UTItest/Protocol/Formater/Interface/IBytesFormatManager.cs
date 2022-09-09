using System.Linq;

namespace UTI
{
    /// <summary>
    /// 传输字节格式实例管理器
    /// </summary>
    public interface IBytesFormatManager
    {
        /// <summary>
        /// 获取格式化器定义
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IBytesFormater GetFormatDefined(byte type);
        /// <summary>
        /// 克隆格式化器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IBytesFormater Clone(IBytesFormater obj);
        /// <summary>
        /// 克隆格式化器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IBytesFormater[] Clone(IBytesFormater[] obj);
    }
}


