namespace UTI
{
    public enum SessionType : byte
    {
        CheckIfConnectable = 0,
        Initilize = 1,
        Connect = 2,
        Connecting = 3,
        Connected = 4,

        Disconnect = 100,
        ErrorDisconnect = 101,
        TimeOutDisconnect = 102,

        Check = 203,
        CheckBack = 204,
    }
}


