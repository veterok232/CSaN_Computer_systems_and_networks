using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class TCPServer
    {
        IPEndPoint serverIP;
        private const int MaxConnections = 10;
        public static ManualResetEvent allDone;
        public static ManualResetEvent sendDone;
        public Socket server;
        public Socket client;
        public ServerHandler handler;

        public static StateObject state;
        public static bool isReceive = false;

        public TCPServer(IPAddress ip, int port)
        {
            // получаем адреса для запуска сокета
            this.serverIP = new IPEndPoint(ip, port);
            allDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);
        }

        public void Start()
        {
            // создаем сокет
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                this.server.Bind(serverIP);
                this.server.Listen(MaxConnections);

                Console.WriteLine("Server is running!");
                Console.WriteLine("Wait connections...");

                while (true)
                {
                    allDone.Reset();
                    this.server.BeginAccept(new AsyncCallback(AcceptCallback), this.server);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket client_socket = listener.EndAccept(ar);
                handler = new ServerHandler(client_socket);

                Console.WriteLine("New connection ip = {0}", client_socket.RemoteEndPoint);

                SendDayTime();
                sendDone.WaitOne();
                Console.WriteLine("DayTime is sent to client");
                allDone.Set();

                Console.WriteLine("Connection with {0} is closed.\n", client_socket.RemoteEndPoint);
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendDayTime()
        {
            DateTime date = DateTime.Now;
            string dayTimeStr = date.ToString();
            byte[] buffer = Encoding.ASCII.GetBytes(dayTimeStr);

            handler.Send(buffer);
        }

        public void Stop()
        {
            try 
            {
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch 
            {
            
            }
        }
    }
}
