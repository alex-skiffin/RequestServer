using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using Newtonsoft.Json;
using Server.DataBase;

namespace Server
{
    class RequestProcessor
    {
        private DBProcessor _dbProcessor = new DBProcessor();
        public RequestProcessor()
        {
            // client.Close();

        }

        public void Listen(TcpClient client)
        {
            StringBuilder request = new StringBuilder();
            byte[] bytes = new byte[1024];
            NetworkStream stream = client.GetStream();
            int i = stream.Read(bytes, 0, bytes.Length);
            request.Append(Encoding.ASCII.GetString(bytes, 0, i));
            do
            {
                // Translate data bytes to a ASCII string.
                Console.WriteLine("Received: {0}", request);

                i = stream.Read(bytes, 0, bytes.Length);
                request.Append(Encoding.ASCII.GetString(bytes, 0, i));
            } while (i > 1023);

            Match reqMatch = Regex.Match(request.ToString(), @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            Match methodMatch = Regex.Match(request.ToString(), @"\b\w+");
            string req = request.ToString();
            int sss = req.Length;
            string req2 = request.ToString();

            if (req.Contains("{"))
            {
                req = req.Substring(req.IndexOf("{", StringComparison.Ordinal));
            }
            int countt = req.Length;
            int countt2 = req2.Length;
            string requestUri = reqMatch.Groups[1].Value;
            string requestMethod = methodMatch.Value;
            // Если запрос не удался
            if (reqMatch == Match.Empty)
            {
                Console.WriteLine("Что-то пошло не так\r\n" + requestUri);
                return;
            }
            string info = string.Empty;
            if (requestMethod == "GET")
            {
                Console.WriteLine("Запрос на получение информации");
                info = JsonConvert.SerializeObject(_dbProcessor.GetInfo());
            }
            if (requestMethod == "POST")
            {
                Console.WriteLine("Запрос на запись информации");
                _dbProcessor.AddInfo(req);
            }
            byte[] buffer = Encoding.ASCII.GetBytes(info);
            // Отправим его клиенту
            client.GetStream().Write(buffer, 0, buffer.Length);
            // Получаем строку запроса
            Console.WriteLine(requestMethod);
            Console.WriteLine(requestUri);
            client.Close();
        }
    }
}