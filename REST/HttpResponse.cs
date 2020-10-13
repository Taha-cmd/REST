using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace REST
{
    public class HttpResponse : HttpRequestResponseBase
    {
        
        public string Protocol { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        private string payload = "";
        public HttpResponse(string status = "400", string message = "Bad Request", string protocol = "HTTP/1.1")
        {
            Protocol = protocol;
            Status = status;
            StatusMessage = message;

            Values = new Dictionary<string, string>();
        }

        public void AddHeader(string key, string value)
        {
            if (Values.ContainsKey(key))
                Values[key] = value;
            else
                Values.Add(key, value);
        }

        public void AddPayload(string payload)
        {
            this.payload += payload;
        }

        public override void Display(ConsoleColor color)
        {
            PrintInColor("Protocol", Protocol, color);
            PrintInColor("Status", Status, color);
            PrintInColor("Status Message", StatusMessage, color);
            base.Display(color);
        }

        public void Send(NetworkStream client)
        {
            // using statement auto disposes and flushes the stream
            using (StreamWriter writer = new StreamWriter(client))
            {
                writer.Write($"{Protocol} {Status} {StatusMessage}\r\n");

                foreach (var kvp in Values)
                    writer.Write($"{kvp.Key}: {kvp.Value}\r\n");

                writer.Write($"Content-Length: {payload.Length}\r\n");
                writer.Write("\r\n");
                writer.Write(payload);
                writer.Write("\r\n\r\n");
            }
        }
    }
}
