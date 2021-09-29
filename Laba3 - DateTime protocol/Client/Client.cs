using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.Security;

namespace Client
{
    class Client
    {
        private const int DAYTIME_PORT = 13;

        static void Main()
        {
            SslStream
            Console.Write("Enter server IP-address: ");
            string ip = Console.ReadLine();
            IPAddress serverIP;
            if (!IPAddress.TryParse(ip, out serverIP))
            {
                Console.WriteLine("Некорректный IP адрес сервера!");
                return;
            }

            Console.WriteLine("Press Enter to connect server...");
            Console.ReadLine();
            Console.WriteLine("Connecting to server...");

            var client = new TCPClient(serverIP, DAYTIME_PORT);
            client.Connect();

            while (!client.isReceive)
            {

            }

            Console.WriteLine("Press Enter to disconnect from server...");
            Console.ReadLine();
            client.Disconnect();
            Console.WriteLine("Disconnecting from server...");
        }
    }
}
