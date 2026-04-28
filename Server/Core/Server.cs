using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Nodes;
using System.Threading;
using System.Data;
using Server.Database;

namespace Server.Core
{
    internal class Server
    {
        private HttpListener? listener;
        private Task? listenerThread;
        private bool listenConnections = true;

        private readonly Dictionary<string, string> apiAddresses = [];

        private string[] config = [];

        private readonly CustomerController customerController = new();

        public Server() { }

        internal void Init()
        {
            Console.WriteLine("Loading");
            //UpdateConfig();
            UpdatePrefixes(["http://127.0.0.1:8001/"]);
        }

        internal void UpdateConfig()
        {
            config = Terminal.GetWebConfig();
            Console.WriteLine(config.Length);
            string[] splitS;
            foreach (string line in config)
            {
                splitS = line.Split("=");
                Console.WriteLine(splitS[0]);
                if (splitS[0] == "AllowedPrefixes")
                {
                    UpdatePrefixes(splitS[1].Split("^"));
                }
                else
                {
                    Console.WriteLine("Command not found");
                }
            }
            Console.WriteLine("Config updated");
        }

        // Same as "allowed origins" in django - what ports it's allowed to listen
        internal void UpdatePrefixes(string[] prefixes)
        {
            if (listener != null) // Execute if server already running
            {
                Console.WriteLine("Warning: The HttpServer already launched \n");
                Console.WriteLine("Updating new prefixes...");


                lock (listener.Prefixes) // Blocks async access to listener.Prefixes until process is done
                {
                    listener.Prefixes.Clear();
                    apiAddresses.Clear();
                    foreach (string s in prefixes)
                    {
                        listener.Prefixes.Add(s);
                        listener.Prefixes.Add(s + @"api/");
                        apiAddresses.Add(s + @"api/", "");
                    }
                }

                Console.WriteLine("Success: Prefixes updated \n");
                return;
            }

            // Execute on first startup

            listener = new HttpListener();
            foreach (string s in prefixes)
            {
                Console.WriteLine(s);
                listener.Prefixes.Add(s);
                Console.WriteLine(s + @"api/");
                listener.Prefixes.Add(s + @"api/");

                apiAddresses.Add(s + @"api/", "");
            }
            listenConnections = true;
            Console.WriteLine("Success: Prefixes loaded \n");
            listenerThread = Task.Run(() => Listening());
        }

        private void Listening() // Process of listening and responding to connections
        {

            if (listener == null) { return; }

            listener.Start();
            //List<Task> listOfRequests = [];
            HttpListenerContext context;
            while (listenConnections)
            {
                try
                {
                    Console.WriteLine("Awaiting");
                    context = listener.GetContext(); // Waiting for request. BLOCKING thread
                    Task.Run(() => Respond(context)); // Responding to request. NOT blocking thread
                    // Is NOT equivalent of // Task.Run(() => listener.GetContext());
                    Console.WriteLine("Connected");
                }
                catch (Exception e)
                {
                    Terminal.PrintException(e);
                }
            }
        }


        private void Respond(HttpListenerContext context) // Respond to request
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                StreamReader streamReader = new(request.InputStream, request.ContentEncoding);
                Stream output;
                byte[] buffer;
                Console.WriteLine(request.UserHostAddress);
                try
                {
                    string debugText = streamReader.ReadToEnd();
                    Encoding encoding = request.ContentEncoding;
                    Console.WriteLine($"{debugText}");
                    Console.WriteLine(!string.IsNullOrEmpty(debugText));
                    if (!string.IsNullOrEmpty(debugText))
                    {
                        JsonDocument clientData = JsonDocument.Parse(debugText);
                        Console.WriteLine(debugText);
                        //Dictionary<string, string> sd = clientData.RootElement.Deserialize<Dictionary<string, string>>()!;
                        // buffer = Encoding.UTF8.GetBytes(ParseData(sd));
                        buffer = Encoding.UTF8.GetBytes(customerController.ParseData(clientData.RootElement.Deserialize<Dictionary<string, string>>()!));
                        response.ContentEncoding = request.ContentEncoding;
                        response.ContentLength64 = buffer.Length;
                        output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close(); // "Close()" sends data to a client
                        Console.WriteLine("Data sent");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Terminal.PrintException(e);
                }

                buffer = SVTerminal.WebPageBytes;
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = buffer.Length;
                output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                Console.WriteLine("No data has been recieved");
            }
            catch (Exception e)
            {
                Terminal.PrintException(e);
            }
        }

        internal void AllowListenConnections() { listenConnections = true; }
        internal void DisableListenConnections()
        {
            listenConnections = false;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            listenerThread.WaitAsync(new CancellationToken(true));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Console.WriteLine("Server shutted down");
        }
    }
}
