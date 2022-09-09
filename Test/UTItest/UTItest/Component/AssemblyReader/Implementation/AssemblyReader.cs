using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UTI
{
    /// <summary>
    /// 集合反射获取类型
    /// </summary>
    /// <typeparam name="TScope">集合</typeparam>
    /// <typeparam name="TFind">目标类型</typeparam>
    public abstract class AssemblyReader<TScope, TFind> : IAssemblyReader where TFind : class
    {
        public static List<Type> AllTypes { get; } = new List<Type>();
        public static List<Type> FindedTypes { get; } = new List<Type>();
        public static List<TFind> FindedInstance { get; } = new List<TFind>();
        public void FindTypes()
        {
            AllTypes.Clear();
            FindedTypes.Clear();
            FindedInstance.Clear();

            AllTypes.AddRange((typeof(TScope)).Assembly.GetTypes());
            foreach (var item in AllTypes.ToArray())
            {
                if (
                    item.IsClass
                    && (!item.IsInterface)
                    && (!item.IsAbstract)
                    && (typeof(TFind)).IsAssignableFrom(item)
                    )
                {
                    FindedTypes.Add(item);
                    OnTypesFinded(item);
                    try
                    {
                        var instance = Activator.CreateInstance(item);
                        if (instance != null && instance is TFind)
                        {
                            FindedInstance.Add(instance as TFind);
                            OnInstanceFinded(instance);
                            OnInstanceFinded(instance as TFind);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"AssemblyReader Error ! : {typeof(TFind).FullName}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 查找到了该类型
        /// </summary>
        /// <param name="type"></param>
        public virtual void OnTypesFinded(Type type)
        {

        }
        /// <summary>
        /// 查找到了该类型 并生成了它的实例
        /// </summary>
        /// <param name="instance"></param>
        public virtual void OnInstanceFinded(TFind instance)
        {

        }
        /// <summary>
        /// 查找到了该类型 并生成了它的实例
        /// </summary>
        /// <param name="instance"></param>
        public virtual void OnInstanceFinded(object instance)
        {

        }
    }
}


