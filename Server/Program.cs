using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 7070;
            if (args.Length > 0)
                port = Int16.Parse(args[0]);
            var server = new ServerProcessor(port);
            Task listen = new Task(() => server.Start());
            listen.Start();
            Console.WriteLine("Сервер запущен на порту {0}", port);
            Console.WriteLine("Для остановки сервера нажмите ENTER...");
            Console.ReadLine();
        }

    }
}
