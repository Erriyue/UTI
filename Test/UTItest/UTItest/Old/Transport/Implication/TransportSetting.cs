namespace UTI
{
    public class TransportSetting
    {
        //最大单次传输字节数
        public static int BufferSize = 65535;

        //连接超时时间,超过60秒客户端还没发消息视为掉线
        public static float TimeoutTime = 300;
    }
}
