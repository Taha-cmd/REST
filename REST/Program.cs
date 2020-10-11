using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace REST
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer server = new HttpServer(8080);

            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptClient();
                Thread requestHandlingThread;

                try
                {
                    requestHandlingThread = new Thread(() => server.HandleClient(client));
                    requestHandlingThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    } 
}
