using System;
using System.Collections.Generic;
using System.Text;

namespace REST
{
    class RequestContext
    {
        public RequestContext(string requestHeaderString)
        {
            requestHeader = requestHeaderString;
            KeyValuePairs = new Dictionary<string, string>();
        }

        public void ParseHeader()
        {
            string[] lines = requestHeader.Split("\n");

            // first line has format METHOD ROUTE PROTOCOL
            string[] tokens = lines[0].Split(' ');
            KeyValuePairs.Add("Method", tokens[0]);
            KeyValuePairs.Add("Route", tokens[1]);
            KeyValuePairs.Add("Protocol", tokens[2]);

            // rest of lines has format key: value
            foreach (string line in lines)
            {
                if (line.Contains(':'))
                {
                    int splitIndex = line.IndexOf(':');
                    string key = line.Substring(0, splitIndex);
                    string value = line.Substring(splitIndex + 1).Trim();
                    KeyValuePairs.Add(key, value);
                }

            }
        }

        public void ParseBody(string requestBodyString)
        {
            requestBody = requestBodyString;
            KeyValuePairs.Add("Payload", requestBody);
        }

        public string[] ParseRoute()
        {
            return KeyValuePairs["Route"].Split("/");
        }

        public void Display()
        {
            Console.Write('\n');

            foreach (KeyValuePair<string, string> kvp in KeyValuePairs)
            {
                PrintInColor(kvp.Key, kvp.Value);
            }

            Console.Write('\n');
        }

        private void PrintInColor(string key, string value, ConsoleColor color = ConsoleColor.Green)
        {
            Console.Write(key + ": ");
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private string requestHeader;
        private string requestBody;
        public Dictionary<string, string> KeyValuePairs { get; set; }

    }
}
