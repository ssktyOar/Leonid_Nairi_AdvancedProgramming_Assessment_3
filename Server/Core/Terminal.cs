using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    internal static class Terminal
    {
        private static bool _consoleMainThread_ = true;
        private static readonly Dictionary<string, Action> actions = [];

        private static readonly string currentDirectoryPath = Directory.GetCurrentDirectory();

        // Config files
        private static readonly string configDirectoryPath = Directory.GetCurrentDirectory() + "\\Configs";
        private static readonly string webConfigPath = Directory.GetCurrentDirectory() + "\\Configs\\WebConfig.txt";
        private static readonly string dbConfigPath = Directory.GetCurrentDirectory() + "\\Configs\\DBConfig.txt";
        private static string[] webConfigFile = [];
        private static string[] dbConfigFile = [];

        // Main thread, can exist only one at a time
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        internal static void TerminalConsole()
        {
            Init();
            SVTerminal.Init();
            string s;
            while (_consoleMainThread_) 
            {
                Console.WriteLine("\n\n");
                s = Console.ReadLine() + "";
                Console.WriteLine("\n\n");

                ParseInput(s);
            }

            Console.WriteLine("Enter any symbol to close console");
            Console.ReadLine();
        }

        private static void ParseInput(string input)
        {
            List<string> i = input.Split(' ').ToList();

            if (i.Count < 2)
            {
                Console.WriteLine("Command must contain at least one prefix");
                return;
            }
            string prefix = i[0];
            i.RemoveAt(0);
            if (prefix == "CT")
            {
                if (actions.TryGetValue(i[0], out Action? value)) { value(); } else { Console.WriteLine("Command not found"); }
            }
            else if (prefix == "SV") { SVTerminal.TerminalConsole(i); }
            //else if (prefix == "DB") { DBTerminal.TerminalConsole(i); }
            else
            {
                Console.WriteLine("Terminal not found");
            }
        }

        private static void Init()
        {
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("[CT] Init");
            Console.WriteLine("__________________________________________________________________________");
            LoadConfigs();

            Console.WriteLine("Loading actions...");

            actions.Add("LoadConfigs", LoadConfigs);
            actions.Add("ShutDown", ShutDown);
            actions.Add("PrintCurrentDirectory", PrintCurrentDirectory);
            actions.Add("PrintConfigDirectory", PrintConfigDirectory);
            actions.Add("Help", Help);

            Console.WriteLine("Action loading is completed...");
            Console.WriteLine("__________________________________________________________________________");
            Help();
        }

        internal static string GetCurrentDirectory() { return currentDirectoryPath; }

        internal static string GetConfigDirectoryPath() { return configDirectoryPath; }

        private static void ShutDown()
        {
            _consoleMainThread_ = false;
        }

        private static void Help()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Each command consists of two parts: prefix and the command itself");
            Console.WriteLine("Prefix - is the type of terminal you want to give a commant");
            Console.WriteLine("Each Terminal have their own set of commands");
            Console.WriteLine();
            Console.WriteLine("Currently exists 3 terminals:");
            Console.WriteLine("CT - Core/Central Terminal");
            Console.WriteLine("SV - Server Terminal, responsible for networking");
            Console.WriteLine("DB - Database Terminal, responsible for connecting to database");
            Console.WriteLine();
            Console.WriteLine("Example of full command below:");
            Console.WriteLine("CT Help");
            Console.WriteLine("__________________________________________________________________________");

            Console.WriteLine("\n\n");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Core Terminal [CT] Help");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine("List of commands:");
            Console.WriteLine("LoadConfigs - load or update configs fron configdirectory; updates configs in sub-terminals as well");
            Console.WriteLine("PrintCurrentDirectory - shows current directory of the app");
            Console.WriteLine("PrintConfigDirectory - shows where configuration files located");
            Console.WriteLine("Help - list of commands");
            Console.WriteLine("ShutDown - shut down server");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("\n\n");

            SVTerminal.Help();
            //DBTerminal.Help();
        }

        private static void LoadConfigs()
        {
            Console.WriteLine("Loading configs...");
            webConfigFile = LoadFile(webConfigPath);
            dbConfigFile = LoadFile(dbConfigPath);
            Console.WriteLine("Configs are loaded");
            SVTerminal.UpdateConfigs();
        }

        private static string[] LoadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("\n\n");
                    Console.WriteLine("Requested File doesn't exists!");
                    Console.WriteLine("Expected path: " + path);
                    Console.WriteLine("\n\n");
                    return [];
                }
                
                var s = File.ReadAllLines(path);
                Console.WriteLine("File loaded: " + s.Length);
                return s;
            }
            catch (Exception e) { PrintException(e); return []; }
        }

        internal static string[] GetWebConfig() { return webConfigFile; }
        internal static string[] GetDBConfig() { return dbConfigFile; }

        internal static void PrintCurrentDirectory()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Current directory:");
            Console.WriteLine(currentDirectoryPath);
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("\n\n");
        }

        internal static void PrintConfigDirectory()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Config directory:");
            Console.WriteLine(configDirectoryPath);
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("\n\n");
        }

        public static void PrintException(Exception e) // Method to conveniently print exceptions
        {
            Console.WriteLine("\n________________________________________________");
            Console.WriteLine(e.HResult + ": " + e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine("\n________________________________________________\n");
        }
    }
}
