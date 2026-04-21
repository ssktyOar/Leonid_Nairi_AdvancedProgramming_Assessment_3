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
using System.Net.Http;
using System.Net.Http.Json;
using Godot;

namespace BankAppClient.Core
{
    internal class SVTerminal
    {
        private static readonly System.Net.Http.HttpClient client = new();
        private static readonly string address = "http://127.0.01:8001/";
        //private static string data = "";

        public static string JsonString(string key, string[] value) // { "key": "value" }
        {
            string s = "[";
            for (int i = 0; i < value.Length - 1; i++)
            {
                s = s + value[i] + ",";
            }
#pragma warning disable IDE0056 // Use index operator
            s = s + value[value.Length - 1] + "]";
#pragma warning restore IDE0056 // Use index operator
            return "{\"" + key + "\": \"" + s + "\"}";
        }



        public static string JsonString(string key, string value) // { "key": "value" }
        {
            return "{\"" + key + "\": \"" + value + "\"}";
        }

        public static string JsonString((string, string)[] data) // { "key1": "value1", "key2": "value2", <...>}
        {
            Dictionary<string, string> d = [];
            foreach (var item in data)
            {
                d.Add(item.Item1, item.Item2);
            }
            return JsonSerializer.Serialize(d);
        }

        public static string JsonString(Dictionary<string, string> data) // { "key1": "value1", "key2": "value2", <...>}
        {
            return JsonSerializer.Serialize(data);
        }

        internal static string TestRequest()
        {
            try
            {
                            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                    {
                        requestType = "test"
                    }),
                Encoding.UTF8,
                "application/json");

            var cl = client.Send(new HttpRequestMessage(HttpMethod.Post, address) { Content = jsonContent });
            Stream s = cl.Content.ReadAsStream();
            byte[] b = new byte[s.Length];
            s.Read(b);
            string result = Encoding.UTF8.GetString(b);
            return result;
            }
            catch (Exception e)
            {
                PrintException(e);
                return "{\"result\": \"failure\"}";
            }
        }

        internal static Dictionary<string,string> Request(Dictionary<string, string> data)
        {
            try
            {
                using StringContent jsonContent = new(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");
                GD.Print(JsonString(data));
                var cl = client.Send(new HttpRequestMessage(HttpMethod.Post, address) { Content = jsonContent });
                Stream s = cl.Content.ReadAsStream();
                byte[] b = new byte[s.Length];
                s.Read(b);
                string result = Encoding.UTF8.GetString(b);
                GD.Print(result);
                return JsonDocument.Parse(result).Deserialize<Dictionary<string,string>>();
            }
            catch (Exception e)
            {
                PrintException(e);
                return null;
            }
        }
        internal static bool TestConnection()
        {
            var data = JsonDocument.Parse(TestRequest()).Deserialize<Dictionary<string, string>>();

            if (data.TryGetValue("result", out string value))
            {
                return value == "success";
            }
            else { return false; }
        }

        public static void PrintException(Exception e) // Method to conveniently print exceptions
        {
            Godot.GD.Print("\n________________________________________________");
            Godot.GD.Print(e.HResult + ": " + e.Message);
            Godot.GD.Print(e.StackTrace);
            Godot.GD.Print("\n________________________________________________\n");
        }
    }
}