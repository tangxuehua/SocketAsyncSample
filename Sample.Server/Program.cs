using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ECommon.TcpTransport;

namespace Sample.Server
{
    class Program
    {
        private const Int32 DEFAULT_PORT = 9900;
        public static int _messageNumber;
        public static Stopwatch _watch;
        public static bool _sendReply = false;  //如果要发送回复，则把该变量改为true即可

        static void Main(string[] args)
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), DEFAULT_PORT);
            var serverListener = new TcpServerListener(serverEndPoint);
            serverListener.StartListening(OnConnectionAccepted, "Normal");

            Console.WriteLine("Server started...");
            Console.ReadLine();
        }

        private static void OnConnectionAccepted(IPEndPoint endPoint, Socket socket)
        {
            var connection = TcpConnection.CreateAcceptedTcpConnection(Guid.NewGuid(), endPoint, socket, true);
            Console.WriteLine("TCP connection accepted: [{0}, L{1}, {2:B}].", connection.RemoteEndPoint, connection.LocalEndPoint, connection.ConnectionId);

            new AcceptedConnection(connection).StartReceiving();
        }
    }
}
