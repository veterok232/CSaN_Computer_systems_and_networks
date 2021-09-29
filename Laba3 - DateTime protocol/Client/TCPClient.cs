using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class TCPClient
    {
        private Socket client;
        private IPEndPoint serverIP;
        private IPEndPoint clientIP;
        public static ManualResetEvent receiveDone;
        public string serverResponse = "";
        public bool isReceive = false;

        public TCPClient(IPAddress ip, int port)
        {
            this.serverIP = new IPEndPoint(ip, port);
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            receiveDone = new ManualResetEvent(false);
        }

        public void Connect()
        {
            try
            {
                this.client.BeginConnect(this.serverIP, new AsyncCallback(ConnectCallBack), this.client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось установить соединение..");
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            var handler = (Socket)ar.AsyncState;
            this.client.EndConnect(ar);
            Console.WriteLine("Сonnected to {0}",
                client.RemoteEndPoint.ToString());
            
            Receive();
            receiveDone.WaitOne();
        }

        public void Disconnect()
        {
            this.client.BeginDisconnect(false, new AsyncCallback(DisconnectCallBack), this.client);
        }
        private void DisconnectCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndDisconnect(ar);
            Console.WriteLine("Соединение разорвано...");
        }

        public void Send(byte[] buffer)
        {
            try
            {
                this.client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), this.client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось отправить данные..");
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndSend(ar);
        }

        private void Receive()
        {
            ClientState state = new ClientState();
            state.workSocket = this.client;

            state.workSocket.BeginReceive(state.buffer, 0, ClientState.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                string cmd = String.Empty;
                ClientState state = ar.AsyncState as ClientState;
                Socket handler = state.workSocket;

                if (handler.Connected)
                {
                    //количество принятых байт
                    int bytes = handler.EndReceive(ar);
                    if (bytes > 0)
                    {
                        //сохраняем принятые данных в классе
                        if (state.buffer.Length == 0)
                            //если приняли пустую строку, то принимаем дальше
                            handler.BeginReceive(state.buffer, 0, ClientState.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
                    }

                    cmd = Encoding.ASCII.GetString(state.buffer);
                    serverResponse = cmd;
                    //serverResponse = serverResponse.TrimEnd(new char[] { '\0' });
                    Console.WriteLine("Server response: {0}\n", serverResponse);
                    receiveDone.Set();
                    isReceive = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
