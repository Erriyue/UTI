using System;
using System.Collections.Generic;

namespace UTI
{

    /// <summary>
    /// 通用JSON结构数据报转换器
    /// 
    /// 优点: 非常简易通用
    /// 缺点: 无法转换非常用的值类型对象 比如自定义Struct类型
    /// </summary>
    public class JsonObjectConverter : IObjectConverter
    {
        private IObjectSpliter Spliter;

        /// <summary>
        /// 构造JSON结构数据报转换器 (默认形式)
        /// 使用默认数据报分割器
        /// </summary>
        public JsonObjectConverter()
        {
            Spliter = new ObjectSpliter();
        }
        /// <summary>
        /// 构造JSON结构数据报转换器
        /// </summary>
        /// <param name="spliter"></param>
        public JsonObjectConverter(IObjectSpliter spliter)
        {
            Spliter = spliter;
        }

        public string ObjectsToString(object[] args)
        {
            List<string> convertsArgs = new List<string>();
            List<string> typeNames = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                typeNames.Add(args[i].GetType().FullName);
            }
            convertsArgs.Add(typeNames.ToJson());
            convertsArgs.Add(args.ToJson());
            return Spliter.MergeArgs(convertsArgs.ToArray());
        }
        public object[] StringToObjects(string message)
        {
            var splitedMessage = Spliter.SplitArgs(message);
            List<string> typeNames = new List<string>(JsonReader.Read<string[]>(splitedMessage[0]));
            object[] Args = JsonReader.Read<object[]>(splitedMessage[1]);
            var data = LitJson.JsonMapper.ToObject(splitedMessage[1]);
            for (int i = 0; i < typeNames.Count; i++)
            {
                var obj = data[i];
                var objjson = obj.ToString();
                var rawjson = Args[i].ToString();
                if (rawjson != objjson)
                {
                    Args[i] = JsonReader.Read(Type.GetType(typeNames[i]), data[i].ToJson());
                }
            }
            return Args;
        }
    }

}
