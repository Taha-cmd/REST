using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace REST
{
    class HttpResponse
    {
        public Dictionary<string, string> Values { get; private set; }
        private string payload = "";

        public string Protocol { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        public HttpResponse(string status = "400", string message = "Bad Request", string protocol = "HTTP/1.0")
        {
            Protocol = protocol;
            Status = status;
            StatusMessage = message;

            Values = new Dictionary<string, string>();
        }

        public void AddHeader(string key, string value)
        {
            Values.Add(key, value);
        }

        public void AddPayload(string payload)
        {
            this.payload += payload;
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
