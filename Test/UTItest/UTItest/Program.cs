using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UTI;


namespace UTI
{
    public class PosClass
    {
        public int x;
        public int y;
        public int z;
    }
    public struct PosStruct
    {
        public int x;
        public int y;
        public int z;
    }
    public class PosStructConverter : ISpecificObjectConverter, IJsonParser, IJsonReader
    {
        public Type DataType => typeof(PosStruct);
        public string DataMark => "posstruct";
        public object JsontoObject(string str)
        {
            var obj = JsonReader.Read<PosClass>(str);
            return new PosClass() { x = obj.x, y = obj.y, z = obj.z };
        }
        public string ObjecttoJson(object obj)
        {
            var posstrcut = (PosStruct)obj;
            return new PosClass() { x = posstrcut.x, y = posstrcut.y, z = posstrcut.z }.ToJson();
        }
    }
}


namespace UTItest
{
    public class Program
    {
        private static IRUDPClient Client = null;
        private static IRUDPServer Server = null;
        public static event Action<string> OnInputLine;
        public static Random r = new Random();
        private static async void OnSend(string obj)
        {
            //开启客户端
            if (obj.Contains("clientstart"))
            {
                obj = obj.Replace("clientstart", "").Trim();
                if (IPEndPoint.TryParse(obj, out var ip))
                {
                    ClientStart(ip);
                }
                else
                {
                    Log.Error("IP 输入错误!");
                }
            }
            if (Client != null)
            {
                //连接服务器
                if (obj.Contains( "connecttoserver"))
                {
                    obj = obj.Replace("connecttoserver", "").Trim();
                    if (IPEndPoint.TryParse(obj, out var ip))
                    {
                        var r1 = await Client.ConnectServer(ip);
                        Log.Error("Connect " + r1.ToString());
                    }
                    else
                    {
                        Log.Error("IP 输入错误!");
                    }
                }
                //断开服务器
                if (obj == "disconnectserver")
                {
                    var server = Client.GetServer();
                    if(server != null)
                    {
                        Client.DisconnectServer(server.IP);
                    }
                }
                //发送消息
                if (obj.Contains("send"))
                {
                    Client.SendMessageToServer((byte)r.Next(0, 99), obj, new object[] { "objs", new PosClass() { x = r.Next(0, 99), y = r.Next(0, 99) }, new PosStruct() { x = r.Next(0, 99), y = r.Next(0, 99) } });
                }
                //查看ping
                if (obj == "ping")
                {
                    if (Client.GetServer() != null)
                        Log.Server("Ping: " + Client.GetServer().Ping.ToString());
                }
            }


            //与上面分开注释编译
            if (obj.Contains("serverstart"))
            {
                obj = obj.Replace("serverstart", "").Trim();
                if (IPEndPoint.TryParse(obj, out var ip))
                {
                    ServerStart(ip);
                }
                else
                {
                    Log.Error("IP 输入错误!");
                }
            }
            if (Server != null)
            {
                //踢掉玩家
                if (obj.Contains("kick"))
                {
                    obj = obj.Replace("kick", "");
                    try
                    {
                        int id = int.Parse(obj);
                        var list = Server.GetClients();
                        if (id >= list.Count)
                        {
                            Log.Error($"数量超过了玩家数量, 最大{list.Count}");
                        }
                        else
                        {
                            var c = list[id];
                            Server.DisconnectClient(c);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //发送消息
                if (obj.Contains("send"))
                {
                    obj = obj.Replace("send", "");
                    try
                    {
                        int id = int.Parse(obj);
                        var list = Server.GetClients();
                        if (id >= list.Count)
                        {
                            Log.Error($"数量超过了玩家数量, 最大{list.Count}");
                        }
                        else
                        {
                            var c = list[id];
                            Server.SendMessageToClient(c, (byte)r.Next(0, 99), obj, new object[] { "objs", new PosClass() { x = r.Next(0, 99), y = r.Next(0, 99) }, new PosStruct() { x = r.Next(0, 99), y = r.Next(0, 99) } });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //展示所有玩家
                if (obj.Contains("list"))
                {
                    int i = 0;
                    foreach (var item in Server.GetClients())
                    {
                        Log.Server($"IP:{item.IP}   ID:{i}  Ping:{item.Ping}");
                        i++;
                    }
                }
            }
            //开启服务器
        }
        public static void ServerStart(IPEndPoint ip)
        {
            Log.Server($"Server Start  {ip.ToString()} !");
            Server = RUDP.StartServer(ip);
            Server.OnClientConnectedEvent += Server_OnClientConnectedEvent; ;
            Server.OnClientDisconnectedEvent += Server_OnClientDisconnectedEvent; ;
            Server.OnReceiveClientMessage += Server_OnReceiveClientMessage; ;
        }
        public static void ClientStart(IPEndPoint ip)
        {
            Log.Server($"Client Start  {ip.ToString()} !");
            Client = RUDP.StartClient(ip);
            Client.OnServerConnectedEvent += Client_OnServerConnectedEvent; ;
            Client.OnServerDisconnectedEvent += Client_OnServerDisconnectedEvent; ;
            Client.OnReceiveServerMessage += Client_OnReceiveServerMessage; ;
        }
        public static async void MainProgress()
        {

        }

        private static void Client_OnServerDisconnectedEvent(IConnection conn)
        {
            Log.Server($"Disconnected {conn.IP}    Ping:{conn.Ping}");
        }

        private static void Client_OnServerConnectedEvent(IConnection conn)
        {
            Log.Server($"Connected {conn.IP}     Ping:{conn.Ping}");
        }

        private static void Client_OnReceiveServerMessage(IConnection conn, byte mark, string message, object[] objects)
        {
            Log.Server($"RECEIVE: {conn.IP}: {mark} {message} {JsonConverter.ToJson(objects)}");
        }





        private static void Server_OnReceiveClientMessage(IConnection conn, byte mark, string message, object[] objects)
        {
            Log.Server($"RECEIVE: {conn.IP}: {mark} {message} {JsonConverter.ToJson(objects)}");
        }

        private static void Server_OnClientDisconnectedEvent(IConnection conn)
        {
            Log.Server($"{conn.IP} is Disconnected    Ping:{conn.Ping}");
        }

        private static void Server_OnClientConnectedEvent(IConnection conn)
        {
            Log.Server($"{conn.IP} is Connected    Ping:{conn.Ping}");
        }





        //private static void Server_OnReceiveClientMessage(IConnection arg1, byte arg2, string arg3, object[] arg4)
        //{
        //   
        //}

        //private static void Server_OnClientDisconnectedEvent(IConnection obj)
        //{
        //    
        //}

        //private static void Server_OnClientConnectedEvent(IConnection obj)
        //{
        //    
        //}

        public static void Main(string[] arg)
        {
            OnInputLine += OnSend;
            //加载资源阶段
            AssemblyReaderLoader.Load();
            //开始主程序
            System.Threading.Tasks.Task.Run(MainProgress);
            //程序出口
            string line;
            while ((line = Console.ReadLine()) != "quit")
            {
                OnInputLine?.Invoke(line);
            }
        }

        //private static void OnClientReceive(string arg1, TransportDatagramType arg2, object[] arg3)
        //{
        //    Log.Server($"OnClientReceive  {arg1} {arg2} {arg3[0].ToJson()}  ");
        //}

        //private static void OnServerReceive(string arg1, TransportDatagramType arg2, object[] arg3)
        //{
        //    Log.Server($"OnServerReceive  {arg1} {arg2} {arg3[0].ToJson()}  ");
        //}
    }
}
