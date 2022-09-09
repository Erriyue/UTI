using System;
using System.Collections.Generic;

namespace UTI
{
    public class BytesFormaterManager : AssemblyReader<BytesFormaterManager, IBytesFormater>, IBytesFormatManager
    {
        public static Dictionary<byte, IBytesFormater> ManagedTypes { get; } = new Dictionary<byte, IBytesFormater>();
        public override void OnInstanceFinded(IBytesFormater instance)
        {
            ManagedTypes.Add(instance.Type, instance);
        }
        public IBytesFormater GetFormatDefined(byte type)
        {
            if (ManagedTypes.ContainsKey(type))
            {
                return Activator.CreateInstance(ManagedTypes[type].GetType()) as IBytesFormater;
            }
            else
            {
                return new ErrorFlag(type);
            }
        }
        public IBytesFormater Clone(IBytesFormater obj)
        {
            var instance = GetFormatDefined(obj.Type);
            instance.SetBytes(obj.GetBytes(), out var l);
            return instance;
        }
        public IBytesFormater[] Clone(IBytesFormater[] obj)
        {
            List<IBytesFormater> list = new List<IBytesFormater>();
            foreach (var item in obj)
            {
                list.Add(Clone(item));
            }
            return list.ToArray();
        }
    }
}


