using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Server.DataBase;

namespace Server
{
    class RequestProcessor
    {
        private readonly DbProcessor _dbProcessor = new DbProcessor();

        public void Listen(TcpClient client)
        {
            string info = string.Empty;
            StringBuilder request = new StringBuilder();
            byte[] bytes = new byte[1024];
            try
            {
                NetworkStream stream = client.GetStream();
                int i = stream.Read(bytes, 0, bytes.Length);
                request.Append(Encoding.ASCII.GetString(bytes, 0, i));

                if (request.ToString().Contains("POST") && !request.ToString().Contains("phone"))
                    do
                    {
                        Console.WriteLine("Received: {0}", request);
                        Thread.Sleep(10);
                        i = stream.Read(bytes, 0, bytes.Length);
                        request.Append(Encoding.ASCII.GetString(bytes, 0, i));
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
                string itemInfo = string.Empty;
                if (requestUri.Split('/').Length > 2)
                    itemInfo = requestUri.Split('/')[2];
                if (reqMatch == Match.Empty)
                {
                    Console.WriteLine("Что-то пошло не так\r\n" + requestUri);
                    return;
                }
                if (requestMethod == "GET")
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    Console.WriteLine("Запрос на получение информации");
                    if (command == "all_phones")
                        info = jsonSerialiser.Serialize(_dbProcessor.GetAllPhone());
                    if (command == "all_contacts")
                        info = jsonSerialiser.Serialize(_dbProcessor.GetAllInfo(itemInfo));
                    if (command == "phone")
                        info = jsonSerialiser.Serialize(_dbProcessor.GetPhone(itemInfo));
                    if (command == "contact")
                        info = JsonConvert.SerializeObject(_dbProcessor.GetInfo(itemInfo));
                    if (command == "")
                        info = JsonConvert.SerializeObject(_dbProcessor.GetPhone());

                }
                if (requestMethod == "POST")
                {
                    Console.WriteLine("Запрос на запись информации");
                    if (req.Contains("{"))
                    {
                        if (command == "phone")
                            _dbProcessor.AddPhoneInfo(req);
                        if (command == "contacts")
                            _dbProcessor.AddAllContactsInfo(req);
                        info = "Записано!";
                    }
                    else
                        Console.WriteLine("Кривой запрос");
                }
                // Отправим его клиенту
                // Получаем строку запроса
                //test
                Console.WriteLine(requestMethod);
                Console.WriteLine(requestUri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            byte[] buffer =
                Encoding.UTF8.GetBytes("HTTP/1.1 200 \nContent-type: text\nContent-Length:" + info.Length +
                                       "\n\n" + info);
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.Close();
        }
        public string Get()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            return  jsonSerialiser.Serialize(_dbProcessor.GetAllPhone());
        }
        public string Get(Guid id)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            return jsonSerialiser.Serialize(_dbProcessor.GetAllInfo((new Phone(){Id = id}).ToString()));
        }
    }
}