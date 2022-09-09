using System;
using System.Net;


namespace UTI
{
    public class Connection : IConnection
    {
        private ConnectionRule Rule = new ConnectionRule();
        private ConnectionSetting Setting = new ConnectionSetting();
        private DateTime LastCheckTime;
        private DateTime LastPingTime;
        private bool PingRecord;
        private int PingTimeOutCount;
        public bool IsServer => Setting.IsServer;
        public IPEndPoint IP { get; }
        public ConnectionState State { get; private set; }
        public int Ping { get; private set; }

        public Connection(IPEndPoint iP)
        {
            IP = iP;
        }
        public void Init(ConnectionRule rule, ConnectionSetting setting)
        {
            this.Rule = rule;
            this.Setting = setting;

            Ping = Rule.MaxPing - 1;
            State = ConnectionState.Init;

            LastCheckTime = DateTime.Now;
            LastPingTime = DateTime.Now;
            PingRecord = false;
            PingTimeOutCount = 0;
        }
        public void BeginCheck()
        {
            PingRecord = true;
            LastPingTime = DateTime.Now;
        }
        public void CheckedCallBack()
        {
            LastCheckTime = DateTime.Now;
            if (PingRecord)
            {
                PingRecord = false;
                var ms = (DateTime.Now - LastPingTime).TotalMilliseconds / 2;
                Ping = ms > 9999 ? (int)9999 : (int)ms;
                if (Ping > Rule.MaxPing)
                {
                    PingTimeOutCount++;
                }
                else
                {
                    PingTimeOutCount = 0;
                }
            }
        }
        public void SetState(ConnectionState state)
        {
            State = state;
        }
        public bool IsTimeOut()
        {
            bool isChecktimeout = (DateTime.Now - LastCheckTime).TotalMilliseconds > Rule.MaxCheckDelayMs;
            bool isPingtimeout = this.PingTimeOutCount > Rule.MaxTimeOutCount;
            return isChecktimeout || isPingtimeout;
        }
    }
}


