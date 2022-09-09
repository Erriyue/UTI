## 项目框架说明
-------------------------------------------------
- 数据报协议部分
- 传输层部分
- Json/String 组件
- Tool 工具  



### DatagramProtocol 数据报协议部分
-------------------------------------------------
**简要介绍**

本框架的数据报协议是指规定规范数据报的内容和解析方式的一种协议

数据报协议的作用有
* 规定数据报格式
* 规范数据报内容
* 将用户输入转换为符合协议的数据报
* 将符合协议的数据报转换为用户的原始输入

本框架的数据报协议层对**传输层**进行封装代理, 来简化操作和提供更高级更人性化的应用  
本框架的数据报协议层也可以通过重写, 继承, 代理等多种方式进行定制化操作或达成更高效/更高级的操作和应用

-------------------------------------------------
**文件**
``` c#
DatagramConverter/                      //数据报转换
    Converter/                          //转换器
        Specified/                      //特定转换器
            SpecifyConverterLoader.cs               /* 特定类型转换器 - 加载器 */
        Universal/                      //通用转换器
            ClassTypesConverter.cs                  /* 类类型对象转换器 */
    Spliter/                            //分割器
        DatagramSpliter.cs                          /* 数据报分割器 */
    CustomDatagramConverter.cs                      /* 自定义结构数据报转换 */
    JsonDatagramConverter.cs                        /* Json结构数据报转换 */
DatagramProtocolProxy/                  //数据报代理
    UniversalDatagramProtocol.cs                    /* 通用数据报协议 */
    UniversalDatagramProtocolProxy.cs               /* 通用数据报协议代理器 */
```

-------------------------------------------------
**功能和实现**

**UniversalDatagramProtocol** 

通用数据报协议 对Itransport传输层进行代理,
依赖于IDatagramsConverter数据报转换器的实现,
来实现其功能.

**UniversalDatagramProtocolProxy**

通用数据报协议代理器 该代理器可以对UniversalDatagramProtocol进行代理以实现更多功能,
此处作为一个案例类, 可以作为改写/重写/继承的依据.


**JsonDatagramConverter**

Json结构数据报转换 是一个IDatagramsConverter转换装置,
实现了将参数化为Json进行合并和解析,
对于参数的分割方式, 依赖IDatagramSpliter分割器,
对于任意参数的Json化, 依赖 Json/* 目录下的组件.

**CustomDatagramConverter**

自定义结构数据报转换 是一个IDatagramsConverter转换装置,
实现了将参数依次序列号合并和解析,
对于参数的分割方式, 依赖IDatagramSpliter分割器,
对于任意类型的参数, 依赖IUniversalTypesConverter通用类型转换器,
对于指定类型的参数, 依赖ISpecificTypesConverter特定参数转换器.

**DatagramSpliter**

通用数据报分割器 是一个IDatagramSpliter分割器,
实现了参数/参数标记的分割和合并.

**ClassTypesConverter**

类类型对象转换器 是一个IUniversalTypesConverter通用类型转换器,
可以将任意类类型对象序列化为Json格式以及反序列化,
依赖 Json/* 目录下的组件.
*不支持struct对象,仅支持部分常用值类型对象int,float,double,bool...*

**SpecifyConverterLoader**

特定类型转换器 - 加载器 是一个AssemblyReader,
查询类型为ISpecificTypesConverter.





### Transport 传输层部分
-------------------------------------------------
**简要介绍**  
传输层负责以下几个任务:
+ 发送:将协议层封装好的消息,以合适的方式正确地发送给服务器或客户端
+ 接受:将客户端或服务器发来的消息正确合适地接受,并传递给协议层  
---客户端独有:
+ 连接:连接指定服务器
+ 断开连接:中途因为各种各样的问题而要断开与服务器的连接是十分有必要的  
---服务端独有:
+ 开启服务器:想要启动服务器必须要有这一步
+ 关闭服务器:您可以调用此方法来随时关闭服务器


#### 如何使用?  
客户端
---
1. 直接new出一个您想要的目标协议即可比如:
``` C# 
	UDPTransport transport = new UDPTransport("192.168.0.1",2333);
```
以下是可供您选择的所有传输协议:
- UDPTransport
- RUDPTransport
2. 实例化完毕后,调用您想使用的ITransport的方法即可,具体哪些功能请您参考下面关于ITransport的详细介绍
3. 现在您通过OnReceived事件获得了一个原始消息,这时候就要引入协议层登场了
``` C#
	IBasicDatagramProtocol ClientProtocol = new UniversalDatagramProtocol(transport); 这里的transport即为上面您指定的传输协议
	ClientProtocol.OnReceive += OnClientReceive; 

	private static void OnClientReceive(string arg1, DatagramType arg2, object[] arg3)
    {
		//这里,您就可以处理处理完毕后的数据了
    	Log.Server($"OnClientReceive  {arg1} {arg2} {arg3[0].ToJson()}  ");
    }
```
完整代码:
``` C#
		public void MainProgress()
        {
            RUDPTransport trans = new RUDPTransport("192.168.5.41", 10000);
            trans.Connect("111.67.196.202:9999");
            ClientProtocol = new UniversalDatagramProtocol(trans);

            ClientProtocol.OnReceive += OnClientReceive;
        }

        private void OnClientReceive(string arg1, DatagramType arg2, object[] arg3)
        {
            Log.Server($"OnClientReceive  {arg1} {arg2} {arg3[0].ToJson()}  ");
        }
```
至此,一个完整的客户端,即为构建完毕.快去试试吧!

服务端
---
1. 还是同上,new一个您想要的传输协议,注意您的协议需要和客户端一致!
``` C# 
	UDPTransport transport = new UDPTransport("192.168.0.1",2333); 
```
2. 实例化完毕后,直接调用其接口里的方法SetupServer()即可
3. 大功告成!现在静待客户端连接



##### 传输层基类(TransportBase)
	- public static string IP; 配置您的IP
	- public static int Port; 配置您的端口号
	- public static int BufferSize = 65535; 您的缓冲大小,不宜过大,会内存溢出,也不宜过小,遇到超大数据包会报错
	- public static float TimeoutTime = 300; 连接超时时间,过了这个时间自动断开连接,对于服务器来说是单个的客户端,对于客户端来说,则是服务器.单位是秒
##### 传输层接口(ITransport)
	- public bool IsRunning { get; protected set; } 服务器&客户端参数:当前是否正在运行
    - public bool IsClient { get; protected set; } 客户端参数:当前是否作为一个客户端运行着
    - public bool IsServer { get; protected set; } 服务器参数:当前是否作为一个服务器运行着
    - public Action<string, byte, string> OnReceived { get; set; }接受数据事件,参数分别是IP,Type,Messgae	
    --------------------------------------------------------------------------------------------------------------------------------------------
    - public int Send(string _IP, byte _type, string _message); 向指定IP发送数据,本框架下以IP地址来标识一个连接而不是ID,将这里的IP理解为传统意义上的ID或较为常用的Token或Session即可
    - public void Connect(string ip); 向目标IP进行连接,客户端专用
    - public void Disconnect(); 断开目前连接,客户端专用
    - public void SetupServer(); 建立服务器,服务端专用
    - public void ShutdownServer(); 关闭服务器,服务端专用
以下传输协议还请您自由选择:
###### UDP传输协议(UDPTransport)继承自TransportBase和ITransport
基于UDP的经典传输协议,没有任何升级,但是好用,以及够用.
-  特点:速度快,面向对象(编程友好)
-  缺点:可能会丢包,乱序,数据错误(自带的校验码并不能100%保证数据准确)
###### 可靠UDP传输协议RUDPTransport继承自TransportBase和ITransport
基于UDP的一种改进协议,通过引入最大限制为2的32次方个的消息的序号来确认数据的完整性和正确性,以及引入基于时间的消息重传机制,保证您数据的安全.
您可以选择是否允许进行消息排队发送.有时将所有消息一个不漏的发出去不是好事,这可能会导致不必要的结果,比如因为网络波动,点了100次抽卡,如果一个不漏地发出去那么就等于抽了100次卡,这很明显不是想要的结果,建议您看情况选择.
- 特点:速度较快,重面向对象轻面向连接,不存在丢包,乱序,数据错误的情况
- 缺点:带宽消耗过大








### Json/String 组件部分
-------------------------------------------------
**简要介绍**

涉及到数据报的转换, 就要涉及到序列化和反序列化, 目前通用的性能较好的序列化方式就是Json  
本组件 依赖 **LitJson插件** , 对其进行封装和利用, 可以实现通用的序列化和反序列化功能.

-------------------------------------------------
**文件**
``` c#
Attribute/                      //标签
    JsonConverIgnoreAttribute.cs                /*json转换忽略*/
CustomJsonConverter/            //自定义转换器
    JsonParserLoader.cs                         /*json解析器加载*/
    JsonReaderLoader.cs                         /*json读取器加载*/
Interface/                      //json转换器接口
    IJsonConverter.cs                           /*json全转换器*/
    IJsonParser.cs                              /*json解析器*/
    IJsonReader.cs                              /*json读取器*/
TypeConver/                     //类类型转换
    TypeConverDictionary.cs                     /*通用类类型字典*/
    TypeConverStruct.cs                         /*类类型字典项*/
JsonConverter.cs                                /*json序列化*/
JsonReader.cs                                   /*json反序列化*/
StringConverter.cs                              /*string反序列化*/
```
-------------------------------------------------
**功能和实现**

*以下 任意类型 指任意类类型和通用值类型, 不包含自定义struct类型*  
*通用值类型指 int float double bool... 此常用的切可以进行string转换的值类型*

**JsonConverter**

实现任意类型到json的转换,
可以使用IJsonParser进行定制化转换.
*无法自动解析struct类型对象, 但是可以针对性的定制转化方式*


**JsonReader**

实现任意json字符串到其原有类型对象的转换,
可以使用IJsonReader进行定制化解析.
*无法自动读取struct类型对象, 但是可以针对性的定制转化方式*


**StringConverter**

实现通用值类型对象的字符串形式到其对象形式的转换 比如  
int: "1" => 1,  
double: "1.56" => 1.56,  
type: "Int" => typeof(Int32),  
type: "Bool" => typeof(Boolean),  
前提是它可以被转换.  
*类类型对象的转换表详见TypeConverDictionary.cs*


**JsonConverIgnoreAttribute**

对于被标记了 JsonConverIgnore 的字段/属性, StringConverter的json转换操作会忽略其.
``` C#
public string name;
public string gender;
[JsonConverIgnore]
public int p2;//临时变量不需要json化
```





### Tool 工具
-------------------------------------------------
**简要介绍**

框架所提供的工具, 用于实现一些封装 和 自动化功能.

-------------------------------------------------
**文件**
``` C#
AssemblyReader/             // 集合读取器
    AssemblyReader.cs       /*集合类型读取*/
    IAssemblyReader.cs      /*集合读取器接口*/
    AssemblyReaderLoader.cs /*集合读取器加载器*/
Log.cs                      /*Log封装*/
```
-------------------------------------------------
**功能和实现**

**AssemblyReader<TScope,TFind>**

集合读取器,  
提供查询集合内类型的方法, 并可以缓存这些类型和类型生成的实例.  
通过override重写进行使用相关功能.  
*查询和TScope同一个命名空间的类型*  
*返回继承/实现了TFind的类型*


**AssemblyReaderLoader**

集合读取器加载器,  
通过**AssemblyReaderLoader.Load()**,
让在和AssemblyReaderLoader相同命名空间的所有AssemblyReader开始运作.  
集合读取器不会自行进行运作, 而是需要一个集合读取器加载器, 在程序的一开始加载所有的集合读取器, 并调用他们开始运作.
*在 程序入口/第一刻 使用它*


**Log**

调试报错器,  
通过实现其方法, 实现项目内的调试log功能.
