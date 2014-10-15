using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using ECommon.TcpTransport;
using ECommon.TcpTransport.Framing;

namespace Sample.Server
{
    class AcceptedConnection
    {
        private readonly ITcpConnection _connection;
        private IMessageFramer _framer;
        private int _isClosed;

        public AcceptedConnection(ITcpConnection connection)
        {
            _framer = new LengthPrefixMessageFramer();
            _framer.RegisterMessageArrivedCallback(OnMessageArrived);
            _connection = connection;

            _connection.ConnectionClosed += OnConnectionClosed;
            if (_connection.IsClosed)
            {
                OnConnectionClosed(_connection, SocketError.Success);
                return;
            }
        }

        public void StartReceiving()
        {
            _connection.ReceiveAsync(OnRawDataReceived);
        }

        private void OnRawDataReceived(ITcpConnection connection, IEnumerable<ArraySegment<byte>> data)
        {
            try
            {
                _framer.UnFrameData(data);
            }
            catch (PackageFramingException exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
            connection.ReceiveAsync(OnRawDataReceived);
        }

        private void OnMessageArrived(ArraySegment<byte> data)
        {
            var current = Interlocked.Increment(ref Program._messageNumber);
            if (current == 1)
            {
                Program._watch = Stopwatch.StartNew();
            }
            if (current % 10000 == 0)
            {
                Console.WriteLine("received message, size:" + data.Count + ", count:" + current + ", timeSpent:" + Program._watch.ElapsedMilliseconds + "ms");
            }
            if (Program._sendReply)
            {
                _connection.EnqueueSend(_framer.FrameData(data));
            }
        }

        private void OnConnectionClosed(ITcpConnection connection, SocketError socketError)
        {
            if (Interlocked.CompareExchange(ref _isClosed, 1, 0) == 0)
            {
                Console.WriteLine("Connection '{0}' [{1:B}] closed: {2}.", connection.ConnectionId, connection.RemoteEndPoint, socketError);
            }
        }
    }
}
