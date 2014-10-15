using System;

namespace ECommon.TcpTransport.Log
{
    public interface ILogger
    {
        void Flush(TimeSpan? maxTimeToWait = null);

        void Fatal(string format, params object[] args);

        void Error(string format, params object[] args);

        void Info(string format, params object[] args);

        void Debug(string format, params object[] args);

        void Trace(string format, params object[] args);

        void FatalException(Exception exc, string format, params object[] args);

        void ErrorException(Exception exc, string format, params object[] args);

        void InfoException(Exception exc, string format, params object[] args);

        void DebugException(Exception exc, string format, params object[] args);

        void TraceException(Exception exc, string format, params object[] args);
    }
}