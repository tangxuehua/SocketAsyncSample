using System;
using System.Diagnostics;

namespace Sample.Client
{
    class Program
    {
        public static int _messageNumber;
        public static Stopwatch _watch;

        static void Main(string[] args)
        {
            var clientCount = 4;

            for (var index = 0; index < clientCount; index++)
            {
                new SocketClient().SendMessages(1024 * 1, 250000);
            }

            Console.WriteLine("Client started...");
            Console.ReadLine();
        }
    }
}
