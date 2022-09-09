using System.Collections.Generic;
using System.Reflection;
using System;

namespace UTI
{
    /// <summary>
    /// 通用类类型数据参数转换器
    /// </summary>
    public class ClassTypesConverter : IUniversalObjectConverter
    {
        public ClassTypesConverter()
        {

        }

        public (Type type, object arg) ConverToObject(string typemark, string str)
        {
            var findType = Type.GetType(typemark);
            if (findType == null)
            {
                throw new Exception($"Type.GetType() Error in ClassTypesDatagramConverter: '{typemark}' Not Find !");
            }
            if (findType.IsValueType || findType == typeof(string))
            {
                if (!StringConverter.Conver(str, findType))
                {
                    Log.Error($"ClassTypesDatagramConverter: '{findType.Name}' is a ValueType and Cant Conver !");
                    return (findType, Activator.CreateInstance(findType));
                }
                else
                {
                    return (findType, StringConverter.ConverTo(str, findType, findType.IsClass ? null : Activator.CreateInstance(findType)));
                }
            }
            return (findType, JsonReader.Read(findType, str));
        }

        public (string mark, string str) ConverToString(Type type, object obj)
        {
            if (type.IsValueType || type == typeof(string))
            {
                var strout = obj.ToString();
                if (!StringConverter.Conver(strout, type))
                {
                    Log.Error($"ClassTypesDatagramConverter: '{type.Name}' is a ValueType and Cant Conver !");
                    return (type.FullName, strout);
                }
                else
                {
                    return (type.FullName, strout);
                }
            }
            return (type.FullName, obj.ToJson());
        }
    }
}


