using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace REST
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            int PORT = 8080;
            TcpListener listener = new TcpListener(IPAddress.Any, PORT);
            try
            {
                listener.Start();
                Console.WriteLine($"listening on port {PORT}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            string ressourcesPath = Path.Join(Directory.GetCurrentDirectory(), "ressources");
           
            while(true)
            {
                Console.WriteLine("waiting for connections...");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("client connected");

                NetworkStream clientStream = client.GetStream();

                StreamReader reader = new StreamReader(clientStream);
                StreamWriter writer = new StreamWriter(clientStream);
 
                string requestHeader = "";
                string data;

                do
                {
                    //if (!clientStream.DataAvailable) break;
                    data = reader.ReadLine();
                    requestHeader += data + "\n";
                } while ( !string.IsNullOrWhiteSpace(data) );  

                RequestContext requestContext = new RequestContext(requestHeader); // first line is protocol route and method

                // check for exceptions here
                try
                {
                    requestContext.ParseHeader();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("error parsing header: " + ex.Message);
                    writer.Write("HTTP/1.0 400 Bad request\r\n");
                    writer.Flush();
                    writer.Dispose();
                    client.Close();
                    continue;
                }
                
                var KeyValuePairs = requestContext.KeyValuePairs;
                char ASCIIValue;
                string body = "";

                //if method is post or put, that means after the empty line after the header comes the body
                if (KeyValuePairs["Method"] == "PUT" || KeyValuePairs["Method"] == "POST")
                {
                    while (reader.Peek() > 0)
                    {
                        ASCIIValue = Convert.ToChar(reader.Read());
                        body += ASCIIValue.ToString();
                    }
                    requestContext.ParseBody(body);
                }

                string path = FileHandle.ComputePath(ressourcesPath, requestContext.ParseRoute());
                string responseHeader = "";
                string responseBody = "";

                switch (KeyValuePairs["Method"])
                {
                    case "GET":
                        if (File.Exists(path + ".txt"))
                        {
                            responseHeader = "HTTP/1.0 200 OK\r\n";
                            responseBody = File.ReadAllText(path + ".txt");
                        }
                        else if (Directory.Exists(path) && Path.GetDirectoryName(path) == ressourcesPath)
                        {
                            responseHeader = "HTTP/1.0 200 OK\r\n";
                            string[] files = FileHandle.GetFiles(path);

                            if (files.Length == 0)
                            {
                                responseBody = "No messages available";
                            }
                            else
                            {
                                responseBody = "found messages: " + files.Length + "\n\n";

                                foreach(string filePath in files)
                                {
                                    int id = Convert.ToInt32(Path.GetFileNameWithoutExtension(filePath));
                                    responseBody += "message " + id + ":\n\"";
                                    responseBody += File.ReadAllText(filePath);
                                    responseBody += "\"\n\n";
                                }
                            }
                        }
                        else
                        {
                            responseHeader = "HTTP/1.0 404 Not Found\r\n";
                            responseBody = "Ressource not found\n";
                        }
                        break;

                    case "POST": 
                        if(Directory.Exists(path) && Path.GetDirectoryName(path) == ressourcesPath)
                        {
                            if(KeyValuePairs.ContainsKey("Payload") && KeyValuePairs["Payload"].Trim() != "")
                            {
                                responseHeader = "HTTP/1.0 200 OK\r\n";
                                int id = FileHandle.ComputeNewFileName(path);
                                string newFilePath = Path.Join(path, Convert.ToString(id) + ".txt");
                                File.WriteAllText(newFilePath, KeyValuePairs["Payload"]);
                                responseBody = Convert.ToString(id) + "\n";
                            }
                            else
                            {
                                responseHeader = "HTTP/1.0 400 bad request\r\n";
                                responseBody = "-1\n";
                            }
                        }
                        else
                        {
                            responseHeader = "HTTP/1.0 404 Not Found\r\n";
                            responseBody = "fuck off\n";
                        } 
                        break;

                    case "PUT":
                        if(File.Exists(path + ".txt"))
                        {
                            if (KeyValuePairs.ContainsKey("Payload") && KeyValuePairs["Payload"].Trim() != "")
                            {
                                responseHeader = "HTTP/1.0 200 OK\r\n";
                                File.WriteAllText(path + ".txt", KeyValuePairs["Payload"]);
                                responseBody = "updated successfully\n";
                            }
                            else
                            {
                                responseHeader = "HTTP/1.0 400 bad request\r\n";
                                responseBody = "no payload\n";
                            }
                        }
                        else
                        {
                            responseHeader = "HTTP/1.0 404 Not Found\r\n";
                            responseBody = "fuck off\n";
                        }
                        break;

                    case "DELETE":
                        if(File.Exists(path + ".txt"))
                        {
                            responseHeader = "HTTP/1.0 200 Not OK\r\n";
                            responseBody = "deleted successfully\n";
                            File.Delete(path + ".txt");
                        }
                        else
                        {
                            responseHeader = "HTTP/1.0 404 Not Found\r\n";
                            responseBody = "fuck off\n";
                        }

                        break;
                }

                responseHeader += "Server: my shitty laptop\r\n";
                responseHeader += "Data: " + DateTime.Now + "\r\n";
                responseHeader += "Content-Type: text/plain\r\n";
                responseHeader += "\r\n";

                requestContext.Display();
                writer.Write(responseHeader);
                writer.Write(responseBody);

                writer.Flush();
                reader.Dispose();
                writer.Dispose();
                client.Close();
            }

        }
    }
}
