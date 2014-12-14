using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Server.DataBase;

namespace Server
{
    class RequestProcessor
    {
        private readonly DBProcessor _dbProcessor = new DBProcessor();

        public void Listen(TcpClient client)
        {
            StringBuilder request = new StringBuilder();
            byte[] bytes = new byte[1024];
            NetworkStream stream = client.GetStream();
            int i = stream.Read(bytes, 0, bytes.Length);
            request.Append(Encoding.ASCII.GetString(bytes, 0, i));
            do
            {
                Console.WriteLine("Received: {0}", request);
                if (request.ToString().Contains("POST") && i >= 1023)
                {
                    i = stream.Read(bytes, 0, bytes.Length);
                    request.Append(Encoding.ASCII.GetString(bytes, 0, i));
                }
            } while (i > 1023);

            Match reqMatch = Regex.Match(request.ToString(), @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            Match methodMatch = Regex.Match(request.ToString(), @"\b\w+");
            string req = request.ToString();
            if (req.Contains("{"))
            {
                req = req.Substring(req.IndexOf("{", StringComparison.Ordinal));
            }
            string requestUri = reqMatch.Groups[1].Value;
            string command = requestUri.Split('/')[1];
            string requestMethod = methodMatch.Value;

            if (reqMatch == Match.Empty)
            {
                Console.WriteLine("Что-то пошло не так\r\n" + requestUri);
                return;
            }
            string info = string.Empty;
            if (requestMethod == "GET")
            {
                var jsonSerialiser = new JavaScriptSerializer();
                Console.WriteLine("Запрос на получение информации");
                if (command == "all")
                    info = jsonSerialiser.Serialize(_dbProcessor.GetAllInfo());
                else if (command == "this")
                    info = JsonConvert.SerializeObject(_dbProcessor.GetInfo(Guid.Parse(requestUri.Split('/')[2])));
                else
                    info = JsonConvert.SerializeObject(_dbProcessor.GetInfo());
                byte[] buffer = Encoding.ASCII.GetBytes("HTTP/1.1 200 \nContent-type: text\nContent-Length:" + info.Length + "\n\n" + info);
                client.GetStream().Write(buffer, 0, buffer.Length);

            }
            if (requestMethod == "POST")
            {
                Console.WriteLine("Запрос на запись информации");
                if (req.Contains("{"))
                    _dbProcessor.AddInfo(req);
                else
                    Console.WriteLine("Кривой запрос");
            }
            // Отправим его клиенту
            // Получаем строку запроса
            Console.WriteLine(requestMethod);
            Console.WriteLine(requestUri);
            client.Close();
        }
    }
}