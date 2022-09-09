namespace UTI
{
    public class ErrorFlag : IBytesFormater
    {
        public ErrorFlag()
        {
        }

        public ErrorFlag(byte errorType)
        {
            this.ErrorType = errorType;
            Log.Error($"TransportBytes Error Instance at Type:{ErrorType}!");
        }
        public byte Type => byte.MaxValue;
        public int? MaxLength => 1;
        public byte ErrorType { get; set; }


        public byte[] GetBytes()
        {
            return new byte[0];
        }
        public void SetBytes(byte[] bytes,out int length)
        {
            length = 1;
        }
    }
}


