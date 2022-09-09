using LitJson;
using System;
using System.Collections.Generic;


namespace UTI
{
    /// <summary>
    /// json 读取器
    /// </summary>
    public static class JsonReader
    {
        /// <summary>
        /// Json自定义读取器
        /// </summary>
        public static Dictionary<Type, IJsonReader> Reader { get; } = new Dictionary<Type, IJsonReader>();
        /// <summary>
        /// 设置读取器
        /// </summary>
        /// <param name="reader"></param>
        public static void SetJsonReader(IJsonReader reader)
        {
            Reader.Add(reader.DataType, reader);
        }





        public static object Read(Type type, string json, bool usereader = true)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"json 对象{type.Name} 解析错误 空白!");
                return default;
            }
            try
            {
                if (usereader && Reader.ContainsKey(type))
                {
                    var obj = Reader[type].JsontoObject(json);
                    return obj;
                }
                else
                {
                    var obj = JsonMapper.ToObject(json, type);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{json}\n-- json 对象{type.Name} 解析错误!", ex);
                return default;
            }
        }
        public static List<object> ReadList(Type type, string json, bool usereader = true, Predicate<object> valid = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"jsonList 对象{type.Name} 解析错误 空白!");
                return default;
            }
            //Log.Server("读取JSON列表:");
            List<object> List = new List<object>();
            int count = 0;
            int miss = 0;
            try
            {
                JsonData data = JsonMapper.ToObject(json);
                for (int i = 0; i < data.Count; i++)
                {
                    try
                    {
                        var obj = Read(type, data[i].ToJson(), usereader);
                        if (valid != null && !valid(obj))
                        {
                            miss++;
                        }
                        else
                        {
                            List.Add(obj);
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"json 对象{type.Name} 解析错误!", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("json解析错误!", ex);
                Log.Error(json);
            }
            Log.Loading($"JSON ReadList<{(type).Name}> Load Count: {count}");
            if (valid != null)
                Log.Loading($"Miss {miss}");
            return List;
        }

        public static T Read<T>(string json, bool usereader = true)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"json 对象{typeof(T).Name} 解析错误 空白!");
                return default;
            }
            try
            {
                if (usereader && Reader.ContainsKey(typeof(T)))
                {
                    var obj = (T)Reader[typeof(T)].JsontoObject(json);
                    return obj;
                }
                else
                {
                    T obj = JsonMapper.ToObject<T>(json);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{json}\n-- json 对象{typeof(T).Name} 解析错误!", ex);
                return default;
            }
        }
        public static List<T> ReadList<T>(string json, bool usereader = true, Predicate<T> valid = null) where T : new()
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"jsonList 对象{typeof(T).Name} 解析错误 空白!");
                return default;
            }
            //Log.Server("读取JSON列表:");
            List<T> List = new List<T>();
            int count = 0;
            int miss = 0;
            try
            {
                JsonData data = JsonMapper.ToObject(json);
                for (int i = 0; i < data.Count; i++)
                {
                    try
                    {
                        T obj = Read<T>(data[i].ToJson(), usereader);
                        if (valid != null && !valid(obj))
                        {
                            miss++;
                        }
                        else
                        {
                            List.Add(obj);
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"json 对象{typeof(T).Name} 解析错误!", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("json解析错误!", ex);
                Log.Error(json);
            }
            Log.Loading($"JSON ReadList<{(typeof(T)).Name}> Load Count: {count}");
            if (valid != null)
                Log.Loading($"Miss {miss}");
            return List;
        }
    }
}
