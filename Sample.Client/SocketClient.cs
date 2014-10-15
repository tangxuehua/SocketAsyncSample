using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ECommon.TcpTransport;
using ECommon.TcpTransport.Framing;

namespace Sample.Client
{
    class SocketClient
    {
        private const Int32 DEFAULT_PORT = 9900;
        private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromMilliseconds(1000);
        private static int _index;
        private string _connectionName = "SampleConnection" + Interlocked.Increment(ref _index);
        private Guid _connectionId = Guid.NewGuid();
        private ITcpConnection _connection;
        private IMessageFramer _framer;
        private int _isClosed;

        public SocketClient()
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), DEFAULT_PORT);
            var connector = new TcpClientConnector();
            _connection = connector.ConnectTo(_connectionId, remoteEndPoint, ConnectionTimeout, OnConnectionEstablished, OnConnectionFailed);
            _connection.ConnectionClosed += OnConnectionClosed;
            _connection.ReceiveAsync(OnRawDataReceived);
        }

        public void SendMessages(int messageSize, int messageCount)
        {
            var data = new byte[messageSize];
            _framer = new LengthPrefixMessageFramer();
            _framer.RegisterMessageArrivedCallback(OnMessageArrived);
            var segment = _framer.FrameData(new ArraySegment<byte>(data, 0, data.Length));

            for (var i = 0; i < messageCount; i++)
            {
                _connection.EnqueueSend(segment);
            }
        }

        private void OnConnectionEstablished(ITcpConnection connection)
        {
            Console.WriteLine("Connection '{0}' ({1:B}) to [{2}] established.", _connectionName, _connectionId, connection.RemoteEndPoint);
        }
        private void OnConnectionFailed(ITcpConnection connection, SocketError socketError)
        {
            if (Interlocked.CompareExchange(ref _isClosed, 1, 0) == 0)
            {
                Console.WriteLine("Connection '{0}' ({1:B}) to [{2}] failed: {3}.", _connectionName, _connectionId, connection.RemoteEndPoint, socketError);
            }
        }
        private void OnConnectionClosed(ITcpConnection connection, SocketError socketError)
        {
            if (Interlocked.CompareExchange(ref _isClosed, 1, 0) == 0)
            {
                Console.WriteLine("Connection '{0}' [{1}, {2:B}] closed: {3}.", _connectionName, connection.RemoteEndPoint, _connectionId, socketError);
            }
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
                Console.WriteLine("received reply message, size:" + data.Count + ", count:" + current + ", timeSpent:" + Program._watch.ElapsedMilliseconds + "ms");
            }
        }
    }
}
