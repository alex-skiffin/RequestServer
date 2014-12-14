using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class ServerProcessor
    {
        readonly TcpListener _listener;
        public ServerProcessor(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            var reqpro = new RequestProcessor();
            _listener.Start();
            while (true)
            {
                reqpro.Listen(_listener.AcceptTcpClient());
            }
        }

        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        ~ServerProcessor()
        {
            Stop();
        }
    }
}