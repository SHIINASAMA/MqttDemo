using System;

namespace MqttServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Server server = new();
            server.Init();
            Console.ReadLine();
        }
    }
}