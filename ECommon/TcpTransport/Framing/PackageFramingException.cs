﻿using System;
using System.Runtime.Serialization;

namespace ECommon.TcpTransport.Framing
{
    public class PackageFramingException: Exception
    {
        public PackageFramingException()
        {
        }

        public PackageFramingException(string message) : base(message)
        {
        }

        public PackageFramingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PackageFramingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
