using System;

namespace UTI
{
    /// <summary>
    /// 字符串类型转换类
    /// </summary>
    public class TypeConverStruct
    {
        public string Name;
        public Type Type;
        public string Discription;
        public Func<string, bool> StringToType;
        public Func<Type, bool> TypeToString;
        public TypeConverStruct(string name, string discription, Type type)
        {
            Name = name;
            Discription = discription;
            Type = type;
            StringToType = (s) => s.ToLower() == Name.ToLower();
            TypeToString = (t) => t == Type;
        }
        public TypeConverStruct(string name, string discription, Type type, Func<string, bool> stringToType, Func<Type, bool> typeToString)
        {
            Name = name;
            Discription = discription;
            Type = type;
            StringToType = stringToType;
            TypeToString = typeToString;
        }
    }
}
