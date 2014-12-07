using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        readonly TcpListener _listener;
        public Server(int port)
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

        ~Server()
        {
            Stop();
        }
    }
}
