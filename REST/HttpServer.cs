using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace REST
{
    class HttpServer
    {
        private TcpListener listener;

        public bool Running { get; private set; }

        public HttpServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Running = true;
        }

        public TcpClient AcceptClient()
        {
            TcpClient client = listener.AcceptTcpClient();
            return client;
        }

        public void HandleClient(TcpClient client)
        {
            try
            {
                HttpRequest request = new HttpRequest(client.GetStream());
                request.Display();

                RequestHandler handler = new RequestHandler();
                HttpResponse response = handler.HandleRequest(request);

                response.AddHeader("Content-Type", "text");
                response.AddHeader("Server", "my shitty laptop");
                response.AddHeader("Date", DateTime.Today.ToString());

                response.Send(client.GetStream());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
   
        }
    }
}
