using System;

namespace UTI
{
    public class MessageContext
    {
        public int SendCount { get; private set; }
        public DateTime SendTime { get; private set; }
        public IBytesFormater[] Message { get; }

        public MessageContext(IBytesFormater[] message, IBytesFormatManager manager)
        {
            Message = manager.Clone(message);
            Init();
        }

        public void Init()
        {
            this.SendCount = 0;
            this.SendTime = default;
        }
        public void Sended(DateTime time)
        {
            SendTime = time;
            SendCount++;
        }
    }
}


