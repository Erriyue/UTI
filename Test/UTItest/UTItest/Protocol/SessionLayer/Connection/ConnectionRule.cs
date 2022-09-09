namespace UTI
{
    public class ConnectionRule
    {
        public int MaxTimeOutCount = 5;
        public int MaxPing = 1000;
        public int MaxCheckDelayMs = 6000;

        public ConnectionRule()
        {
        }

        public ConnectionRule(int maxTimeOutCount, int maxPing, int maxCheckDelayMs)
        {
            MaxTimeOutCount = maxTimeOutCount;
            MaxPing = maxPing;
            MaxCheckDelayMs = maxCheckDelayMs;
        }
    }
}


