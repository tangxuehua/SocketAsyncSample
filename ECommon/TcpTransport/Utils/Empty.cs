using System;

namespace ECommon.TcpTransport.Utils
{
    public static class Empty
    {
        public static readonly byte[] ByteArray = new byte[0];
        public static readonly string[] StringArray = new string[0];
        public static readonly object[] ObjectArray = new object[0];

        public static readonly Action Action = () => { };
    }
}
