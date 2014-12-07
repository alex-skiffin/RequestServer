using System.Net;
using System.Net.Sockets;

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
            _listener.Start();
            while (true)
            {
                new RequestProcessor(_listener.AcceptTcpClient());
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
