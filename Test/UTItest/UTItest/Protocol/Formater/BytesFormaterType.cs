namespace UTI
{
    /// <summary>
    /// 1 - 254
    /// </summary>
    public enum BytesType : byte
    {
        Header = byte.MinValue,
        Error = byte.MaxValue,

        ReliabilityFlag = 1,
        SessionFlag = 2,
        UniversalTransportFlag = 3,
        UniversalTransportArgs = 4,
    }
}


