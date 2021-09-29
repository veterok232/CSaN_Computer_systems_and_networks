using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private const int DAYTIME_PORT = 37;

        static void Main()
        {
            Console.Write("Enter server IP-address: ");
            string ip = Console.ReadLine();
            IPAddress serverIP;
            if (!IPAddress.TryParse(ip, out serverIP))
            {
                Console.WriteLine("Incorrect server IP-address!");
                return;
            }

            Console.WriteLine("Press Enter to run server...");
            Console.ReadLine();

            var server = new TCPServer(serverIP, DAYTIME_PORT);
            Task.Run(() => server.Start());
            //server.Start();

            Console.WriteLine("Press Enter to stop server...");
            Console.ReadLine();
            server.Stop();
            Console.WriteLine("Server is stopped..");
            Console.ReadLine();
        }
    }
}
