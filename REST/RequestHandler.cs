using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REST
{
    class RequestHandler
    {
        public RequestHandler() { }

        public HttpResponse HandleRequest(HttpRequest request)
        {

            switch (request.Values["Method"].ToUpper())
            {
                case "GET":     return HandleGetRequest(request);
                case "POST":    return HandlePostRequest(request);
                case "PUT":     return HandlePutRequest(request);
                case "DELETE":  return HandleDeleteRequest(request);
            }

            return new HttpResponse("405", "Method Not Allowed");
        }


        private HttpResponse HandleGetRequest(HttpRequest request)
        {
            HttpResponse response = new HttpResponse("200", "OK");
            FileHandle fileHandle = new FileHandle();
            string path = fileHandle.PathWithRoot(fileHandle.ComputePath(request.Values["Route"]));

            if (File.Exists(path + ".txt"))
            {
                response.AddPayload(File.ReadAllText(path + ".txt"));
            }
            else if (Directory.Exists(path) && path == fileHandle.Root)
            {
                response.AddPayload(File.ReadAllText(Path.Join(fileHandle.Root, "help.txt")));
            }
            else if (Directory.Exists(path))
            {
                string[] files = fileHandle.GetFiles(path);

                if (files.Length == 0)
                {
                    response.AddPayload("No messages available");
                }
                else
                {
                    response.AddPayload($"Found messages: {files.Length}\n");

                    foreach (string filePath in files)
                    {
                        int id = Convert.ToInt32(Path.GetFileNameWithoutExtension(filePath));
                        response.AddPayload($"Message {id}:\n");
                        response.AddPayload(File.ReadAllText(filePath));
                        response.AddPayload("\n\n");
                    }
                }
            }
            else
            {
                response.Status = "404";
                response.StatusMessage = "Not Found";
            }

            return response;

        }

        private HttpResponse HandlePostRequest(HttpRequest request)
        {
            HttpResponse response = new HttpResponse("200", "OK");
            FileHandle fileHandle = new FileHandle();
            string path = fileHandle.PathWithRoot(fileHandle.ComputePath(request.Values["Route"]));

            if (Directory.Exists(path))
            {
                if (request.Values["Payload"].Trim() != "")
                {
                    int id = fileHandle.ComputeNewFileName(path);
                    string newFilePath = Path.Join(path, Convert.ToString(id) + ".txt");
                    File.WriteAllText(newFilePath, request.Values["Payload"]);
                    response.AddPayload($"{Convert.ToString(id)}\n");
                }
                else
                {
                    response.StatusMessage = "Bad Request";
                    response.Status = "400";
                    response.AddPayload("No Payload");
                }
            }
            else
            {
                response.StatusMessage = "Not Found";
                response.Status = "404";
            }


            return response;
        }


        private HttpResponse HandlePutRequest(HttpRequest request)
        {
            HttpResponse response = new HttpResponse("200", "OK");
            FileHandle fileHandle = new FileHandle();
            string path = fileHandle.PathWithRoot(fileHandle.ComputePath(request.Values["Route"]));

            if (File.Exists(path + ".txt"))
            {
                if (request.Values["Payload"].Trim() != "")
                {
                    File.WriteAllText(path + ".txt", request.Values["Payload"]);
                    response.AddPayload("Updated Successfully\n");
                }
                else
                {
                    response.StatusMessage = "Bad Request";
                    response.Status = "400";
                    response.AddPayload("No Payload");
                }
            }
            else
            {
                response.StatusMessage = "Not Found";
                response.Status = "404";
            }

            return response;
        }


        private HttpResponse HandleDeleteRequest(HttpRequest request)
        {
            HttpResponse response = new HttpResponse("200", "OK");
            FileHandle fileHandle = new FileHandle();
            string path = fileHandle.PathWithRoot(fileHandle.ComputePath(request.Values["Route"]));

            if (File.Exists(path + ".txt"))
            {
                response.AddPayload("Deleted Successfully\n");
                File.Delete(path + ".txt");
            }
            else
            {
                response.StatusMessage = "Not Found";
                response.Status = "404";
            }

            return response;
        }
    }
}
