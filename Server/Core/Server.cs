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
        private readonly Dictionary<string, string> cookies = []; // key - cookie, data - login
        private readonly Dictionary<string, Customer> cookiesByUsers = []; // key - login, data - cookie
        private readonly Dictionary<string, Customer> customers = []; // key - login, data - customer
        private readonly string jsonResponseSuccess;
        private readonly string jsonResponseFailure;

        private readonly Dictionary<string, string> apiAddresses = [];

        private string[] config = [];

        //

        private readonly decimal newInvestmentRate = 0.05m;
        private readonly decimal newFailedTransactionFee = 5m;
        private readonly decimal newOverdraftLimit = 1000m;

        public Server()
        {
            jsonResponseSuccess = JsonString("result", "success");
            jsonResponseFailure = JsonString("result", "failure");
            List<EverydayAccount> l1 = [];
            l1.Add(new EverydayAccount(true, 5497631m, "TestBankAccount", 1111));
            l1.Add(new InvestmentAccount(true, 5631m, "TestBankAccount", 2222, newInvestmentRate, newFailedTransactionFee));
            l1.Add(new OmniAccount(true, 15631m, "TestBankAccount", 3333, newInvestmentRate, newFailedTransactionFee, newOverdraftLimit));
            Customer c1 = new("1", "1", "TestEmployee", "1", "1@1.com", l1);
            customers.Add(c1.customerID, c1);
            List<EverydayAccount> l2 = [];
            l2.Add(new OmniAccount(false, 631m, "TestBankAccount", 4444, newInvestmentRate, newFailedTransactionFee, newOverdraftLimit));
            l2.Add(new InvestmentAccount(false, 0m, "TestBankAccount", 5555, newInvestmentRate, newFailedTransactionFee));
            Customer c2 = new("2", "1111", "Test", "2", "2@1.com", l2);
            customers.Add(c2.customerID, c2);
        }

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
        private static bool ValidateSession(string cookie, Customer customer)
        {
            return customer.ValidateSession(cookie);
        }

        private string GenerateCookie(string userLogin)
        {
            string s;

            if (cookiesByUsers.ContainsKey(userLogin)) // If there is already cookie of same user, delete old one
            {
                cookies.Remove(cookiesByUsers.GetValueOrDefault(userLogin)!.SessionID);
                cookiesByUsers.Remove(userLogin);
            }
            s = BitConverter.ToString(RandomNumberGenerator.GetBytes(256)); // cryptographically strong random string
            customers.TryGetValue(userLogin, out Customer? customer);
            cookies.Add(s, userLogin);
            cookiesByUsers.Add(userLogin, customer!);
            return s;
        }

        // Generating JSONs to send them to user

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
                        buffer = Encoding.UTF8.GetBytes(ParseData(clientData.RootElement.Deserialize<Dictionary<string, string>>()!));
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

        private string ParseData(Dictionary<string, string> parsedData) // Parse and respond
        {
            string s = "Oh hi";
            try
            {
                if (parsedData.TryGetValue("requestType", out string? requestType))
                {
                    if (requestType == "test")
                    {
                        s = jsonResponseSuccess;
                    }
                    else if (requestType == "cookieTest")
                    {
                        if (CookieTest(parsedData))
                        { s = jsonResponseSuccess; }
                        else { s = jsonResponseFailure; }
                    }
                    else if (requestType == "login")
                    {
                        s = Login(parsedData);
                    }
                    else if (requestType == "logout")
                    {
                        s = Logout(parsedData);
                    }
                    else if (requestType == "changePassword")
                    {
                        s = ChangePassword(parsedData);
                    }
                    else if (requestType == "getInfo")
                    {
                        s = GetInfo(parsedData);
                    }
                    else if (requestType == "editContactData")
                    {
                        s = EditContactData(parsedData);
                    }
                    else if (requestType == "transaction")
                    {
                        s = Transaction(parsedData);
                    }
                    else if (requestType == "createBankAccount")
                    {
                        s = CreateBankAccount(parsedData);
                    }
                    else if (requestType == "editBankAccount")
                    {
                        s = EditBankAccount(parsedData);
                    }
                    else if (requestType == "changeAccountPin")
                    {
                        s = ChangeAccountPin(parsedData);
                    }
                    else if (requestType == "deleteBankAccount")
                    {
                        s = DeleteBankAccount(parsedData);
                    }
                    Console.WriteLine(s);
                    Console.WriteLine(DateTime.Now.ToString());
                    return s;
                }
                foreach (string value in parsedData!.Values)
                {
                    Console.WriteLine(value);
                }
            }
            catch (Exception e)
            {
                Terminal.PrintException(e);
                s = jsonResponseFailure;
            }
            Console.WriteLine(s);
            return s;
        }

        private string Login(Dictionary<string, string> parsedData)
        {
            if (parsedData.TryGetValue("customerLogin", out string? login) && parsedData.TryGetValue("password", out string? password))
            {
                if (customers.TryGetValue(login, out Customer? customer))
                {
                    if (customer.Login(GenerateCookie(login), password))
                    {
                        return JsonString([("result", "success"), ("cookie", customer.SessionID)]);
                    }
                }
            }
            return jsonResponseFailure;
        }

        private string Logout(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData))
            {
                return jsonResponseSuccess;
            }
            return jsonResponseFailure;
        }

        private string GetInfo(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && parsedData.TryGetValue("cookie", out string? cookie))
            {
                return JsonString([("result", "success"), ("data", customers[cookies[cookie]].ToString())]);
            }
            return jsonResponseFailure;
        }

        private string ChangePassword(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("password", out string? password) &&
            parsedData.TryGetValue("newPassword", out string? newPassword))
            {
                if (customers[cookies[cookie]].ChangePassword(cookie, password, newPassword))
                {
                    return jsonResponseSuccess;
                }
            }
            return jsonResponseFailure;
        }

        private string EditContactData(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("name", out string? newName) &&
            parsedData.TryGetValue("phone", out string? newPhone) &&
            parsedData.TryGetValue("email", out string? newEmail))
            {
                if (customers[cookies[cookie]].EditContactData(cookie, newName, newPhone, newEmail))
                {
                    return jsonResponseSuccess;
                }
            }
            return jsonResponseFailure;
        }

        private string Transaction(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("bankAccountID", out string? bankAccountIDString) &&
            parsedData.TryGetValue("pin", out string? pinString) &&
            parsedData.TryGetValue("transactionType", out string? transactionType) &&
            parsedData.TryGetValue("sum", out string? sumString))
            {
                try
                {
                    if (customers[cookies[cookie]].Transaction(cookie, 
                    long.Parse(bankAccountIDString), 
                    short.Parse(pinString), transactionType, Math.Abs(decimal.Parse(sumString))))
                    {
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string CreateBankAccount(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("accountType", out string? accountType) &&
            parsedData.TryGetValue("isBankStaff", out string? isBankStaff) &&
            parsedData.TryGetValue("accountName", out string? accountName) &&
            parsedData.TryGetValue("pin", out string? pinString))
            {
                try
                {
                    if (customers[cookies[cookie]].CreateBankAccount(cookie, 
                    accountType, bool.Parse(isBankStaff), 0m, accountName, short.Parse(pinString),
                    newInvestmentRate, newFailedTransactionFee, newOverdraftLimit)) // 5% investment rate, $5 transaction fee, $1000 overdraft limit
                    {
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string EditBankAccount(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("bankAccountID", out string? bankAccountID) &&
            parsedData.TryGetValue("accountName", out string? accountName))
            {
                try
                {
                    if (customers[cookies[cookie]].EditBankAccount(cookie, 
                    long.Parse(bankAccountID), accountName))
                    {
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string ChangeAccountPin(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("bankAccountID", out string? bankAccountID) &&
            parsedData.TryGetValue("pin", out string? pinString) &&
            parsedData.TryGetValue("newPin", out string? newPinString))
            {
                try
                {
                    if (customers[cookies[cookie]].ChangeAccountPin(cookie, 
                    long.Parse(bankAccountID), short.Parse(pinString), short.Parse(newPinString)))
                    {
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string DeleteBankAccount(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) && 
            parsedData.TryGetValue("cookie", out string? cookie) && 
            parsedData.TryGetValue("bankAccountID", out string? bankAccountID) &&
            parsedData.TryGetValue("pin", out string? pinString))
            {
                try
                {
                    if (customers[cookies[cookie]].DeleteBankAccount(cookie, 
                    long.Parse(bankAccountID), short.Parse(pinString)))
                    {
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string GetRoles(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData))
            {
                return JsonString([("result", "success"), ("data", "[\"Student\", \"Tutor\", \"Admin\", \"Superadmin\"]")]);
            }
            return jsonResponseFailure;
        }

        private bool CookieTest(Dictionary<string, string> parsedData) // Simple check if cookie is valid
        {
            if (parsedData.TryGetValue("cookie", out string? cookie)
                && cookies.TryGetValue(parsedData.GetValueOrDefault("cookie")!, out string? login))
            {
                Console.WriteLine(cookie);
                return ValidateSession(cookie,customers[login]);
            }
            return false;
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
