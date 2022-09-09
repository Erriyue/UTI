using System;
using System.Collections.Generic;

namespace UTI
{
    

    public enum TransportResultType : int
    {
        ConnectionNotExist = -3,
        OutOfBoundry = -2,
        Missed = -1,
        Failed = 0,
        Successed = 1
    }

}


