using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace REST
{
    class HttpRequest : HttpRequestResponseBase
    {
        private string request;
        
        public HttpRequest(NetworkStream clientStream)
        {
            StreamReader reader = new StreamReader(clientStream);

            // read single chars
            // readLine() will block at the empty line
            while (reader.Peek() >= 0)
                request += (char)reader.Read();
               
            Values = new Dictionary<string, string>();
            ParseRequest();
        }

        private void ParseRequest()
        {
            if (string.IsNullOrEmpty(request))
                throw new InvalidDataException();

            string[] lines = request.Split("\r\n");

            // first line has format METHOD ROUTE PROTOCOL
            string[] tokens = lines[0].Split(' ');

            if (tokens.Length != 3)
                throw new InvalidDataException();

            Values.Add("Method", tokens[0]);
            Values.Add("Route", tokens[1]);
            Values.Add("Protocol", tokens[2]);

            // rest of lines has format key: value
            // after the empty line comes the body
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Contains(':'))
                {
                    int splitIndex = lines[i].IndexOf(':');
                    string key = lines[i].Substring(0, splitIndex);
                    string value = lines[i].Substring(splitIndex + 1).Trim();
                    Values.Add(key, value);
                }
                else if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    Values.Add("Payload", "");
                    for (int j = i + 1; j < lines.Length; j++)
                        Values["Payload"] += lines[j] + "\n";

                    break;
                }
            }
        }
    }
}
