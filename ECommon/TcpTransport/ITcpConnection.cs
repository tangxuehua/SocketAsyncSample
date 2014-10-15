using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ECommon.TcpTransport
{
    public interface ITcpConnection
    {
        event Action<ITcpConnection, SocketError> ConnectionClosed;

        Guid ConnectionId { get; }
        IPEndPoint RemoteEndPoint { get; }
        IPEndPoint LocalEndPoint { get; }
        int SendQueueSize { get; }
        bool IsClosed { get; }

        void ReceiveAsync(Action<ITcpConnection, IEnumerable<ArraySegment<byte>>> callback);
        void EnqueueSend(IEnumerable<ArraySegment<byte>> data);
        void Close(string reason);
    }
}