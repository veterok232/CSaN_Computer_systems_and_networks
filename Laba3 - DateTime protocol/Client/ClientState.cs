using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class ClientState
    {
        // Size of receive buffer.  
        public const int bufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[bufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }
}
