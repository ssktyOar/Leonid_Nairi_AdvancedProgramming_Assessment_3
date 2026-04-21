using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Core
{
    internal static class SVTerminal
    {
        private static bool firstInit = true;
        private static bool debugInfo = true;
        internal static byte[] WebPageBytes { private set; get; } = [];

        private static List<string> terminalInput = [];
        private static readonly Server server = new();

        internal static void TerminalConsole(List<string> input)
        {
            terminalInput = input;
            if (terminalInput[0] == "Init")
            {
                Init();
            }
            else if (terminalInput[0] == "UpdateConfig")
            {
                UpdateConfigs();
            }
            else if (terminalInput[0] == "Help")
            {
                Help();
            }
            else
            {
                Console.WriteLine("Command not found");
            }
        }
        internal static void Init()
        {
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("[SV] Init");
            Console.WriteLine("__________________________________________________________________________");
            if (!firstInit)
            {
                Console.WriteLine("Server is already running, use \"SV Update\" instead");
                return;
            }
            firstInit = false;
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("\n\n");
            server.Init();
        }

        internal static void Help()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Server Terminal [SV]");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine("List of commands:");
            Console.WriteLine("UpdateConfigs - load or update configs fron configdirectory; updates configs in sub-terminals as well");
            Console.WriteLine("Help - list of commands");
            Console.WriteLine("\n\n");
        }

        internal static void UpdateConfigs() { server.UpdateConfig(); }

        internal static void SetDebugInfo(bool debug) { debugInfo = debug; }

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
    }
}
