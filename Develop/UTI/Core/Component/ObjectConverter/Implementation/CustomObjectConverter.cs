using System;
using System.Collections.Generic;

namespace UTI
{

    /// <summary>
    /// 通用自定义数据报转换器
    /// 
    /// 优点: 和JSON结构转换器相比, 除了能自定义以外, 没有其他任何优点
    /// 缺点: 默认提供的功能无法对所有的值类型进行转换, 包括int这些常用的值类型
    ///         通过自定义可以实现对任意指定类型进行转换
    /// </summary>
    public class CustomObjectConverter : IObjectConverter
    {
        private List<ISpecificObjectConverter> SpecificConverter { get; }
        private IUniversalObjectConverter UniversalConverter;
        private IObjectSpliter Spliter;

        /// <summary>
        /// 构造自定义结构数据报转换器 (默认形式)
        /// 使用默认类类型转换器
        /// 使用默认数据报分割器
        /// 自动查找特定类型转换器
        /// </summary>
        public CustomObjectConverter()
        {
            SpecificConverter = new List<ISpecificObjectConverter>(SpecifyConverterLoader.FindedInstance);
            UniversalConverter = new ClassTypesConverter();
            Spliter = new ObjectSpliter();
        }
        /// <summary>
        /// 构造自定义结构数据报转换器
        /// </summary>
        /// <param name="spliter">数据报分割器</param>
        /// <param name="universal">通用数据报类型转换器</param>
        /// <param name="specific">特定数据报类型转换器</param>
        public CustomObjectConverter(IObjectSpliter spliter, IUniversalObjectConverter universal, ISpecificObjectConverter[] specific)
        {
            SpecificConverter = new List<ISpecificObjectConverter>(specific);
            UniversalConverter = universal;
            Spliter = spliter;
        }


        public string ObjectsToString(object[] args)
        {
            List<string> convertsArgs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                //获取参数基本信息
                object arg = args[i];
                Type argType = arg.GetType();

                //拆解参数结构
                string argTypemark;
                string argStr;

                var converter = SpecificConverter.Find(e => e.DataType == argType);
                if (converter != null)
                {
                    argStr = converter.ObjecttoJson(arg);
                    argTypemark = converter.DataMark;
                }
                else
                {
                    var resultInfo = UniversalConverter.ConverToString(argType, arg);
                    argStr = resultInfo.str;
                    argTypemark = resultInfo.mark;
                }

                //生成参数序列化
                var argInfo = this.Spliter.MergeDataInfo(argTypemark, argStr);
                convertsArgs.Add(argInfo);
            }
            return Spliter.MergeArgs(convertsArgs.ToArray());
        }
        public object[] StringToObjects(string message)
        {
            var convertsArgs = Spliter.SplitArgs(message);
            List<object> Args = new List<object>();
            for (int i = 0; i < convertsArgs.Length; i++)
            {
                //获取参数反序列化
                var arginfo = this.Spliter.SplitDataInfo(convertsArgs[i]);
                string argTypemark = arginfo.typemark;
                string argstr = arginfo.data;

                //还原参数
                object argobj;
                var converter = SpecificConverter.Find(e => e.DataMark == argTypemark);
                if (converter != null)
                {
                    argobj = converter.JsontoObject(argstr);
                }
                else
                {
                    argobj = UniversalConverter.ConverToObject(argTypemark, argstr).arg;
                }

                //存储参数
                Args.Add(argobj);
            }
            return Args.ToArray();
        }
    }

}
