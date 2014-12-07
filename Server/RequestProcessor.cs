﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
    class RequestProcessor
    {
        public RequestProcessor(TcpClient client)
        {
            string request = "";
            byte[] buffer = new byte[1024];
            int count;
            while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(buffer, 0, count);
                if (request.IndexOf("\r\n\r\n", StringComparison.Ordinal) >= 0 || request.Length > 4096)
                {
                    break;
                }
            }

            Match reqMatch = Regex.Match(request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            Match methodMatch = Regex.Match(request, @"\b\w+");

            string requestUri = reqMatch.Groups[1].Value;
            string requestMethod = methodMatch.Value;
            // Если запрос не удался
            if (reqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                //SendError(Client, 400);
                Console.WriteLine("Что-то пошло не так\r\n"+requestUri);
                return;
            }
            if (requestMethod == "GET")
            {
                Console.WriteLine("Запрос на получение информации");
            }
            if (requestMethod == "POST")
            {
                Console.WriteLine("Запрос на запись информации");
            }

            // Получаем строку запроса
            Console.WriteLine(requestMethod);
            Console.WriteLine(requestUri);
            client.Close();

        }
    }
}