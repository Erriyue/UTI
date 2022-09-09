

namespace UTI
{
    /// <summary>
    /// 协议数据分割器
    /// </summary>
    public interface IObjectSpliter
    {
        /// <summary>
        /// 分割数据标记和本体
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        (string typemark, string data) SplitDataInfo(string content);
        /// <summary>
        /// 合并数据标记和本体
        /// </summary>
        /// <param name="typemark"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string MergeDataInfo(string typemark, string data);


        /// <summary>
        /// 合并多参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        string MergeArgs(string[] args);
        /// <summary>
        /// 分割多参数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        string[] SplitArgs(string content);
    }
}
