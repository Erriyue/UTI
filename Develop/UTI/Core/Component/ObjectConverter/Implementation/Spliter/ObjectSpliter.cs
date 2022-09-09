

namespace UTI
{
    /// <summary>
    /// 通用数据报分割器
    /// </summary>
    public class ObjectSpliter : IObjectSpliter
    {
        public const string DataMark = "-<data>-";
        public const string ArgsMark = "-<args>-";

        public (string typemark, string data) SplitDataInfo(string content)
        {
            var ls = content.Split(new string[] { DataMark }, System.StringSplitOptions.RemoveEmptyEntries);
            return (ls.Length >= 1 ? ls[0] : "null", ls.Length >= 2 ? ls[1] : "null");
        }
        public string MergeDataInfo(string typemark, string data)
        {
            return typemark + DataMark + data;
        }

        public string[] SplitArgs(string content)
        {
            return content.Split(new string[] { ArgsMark }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        public string MergeArgs(string[] args)
        {
            return string.Join(ArgsMark, args);
        }
    }

}

