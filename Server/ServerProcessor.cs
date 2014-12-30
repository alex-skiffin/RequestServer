using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    class ServerProcessor
    {
        readonly HttpListener _listener;
        private Thread _serverThread;
        public ServerProcessor(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format("http://*:{0}/", port));
            Console.WriteLine("Listening on port {0}...", port);
        }

        public void Start()
        {
            var reqpro = new RequestProcessor();

            _listener.Start();

            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                //Создаем ответ
                string requestBody;
                Stream inputStream = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(inputStream, encoding);
                requestBody = reader.ReadToEnd();

                Console.WriteLine("{0} request was caught: {1}",
                                   request.HttpMethod, request.Url);
                string msg = reqpro.Get();
                //Console.WriteLine(msg);

                response.StatusCode = (int)HttpStatusCode.OK;
                byte[] b = Encoding.UTF8.GetBytes(msg);
                context.Response.ContentLength64 = b.Length;
                context.Response.OutputStream.Write(b, 0, b.Length);

                //Возвращаем ответ
                using (Stream stream = response.OutputStream) { }
            }
            /*_listener.Start();
            while (true)
            {
                try
                {

                    _serverThread = new Thread(ServerWorkerFunc) { IsBackground = true, Name = "ListenerThread" };
                    _serverThread.Start();
                    reqpro.Listen(_listener.AcceptTcpClient());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }*/
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