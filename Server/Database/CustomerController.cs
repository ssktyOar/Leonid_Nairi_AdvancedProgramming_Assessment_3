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
using Server.Core;
using Server.Database.Exceptions;

namespace Server.Database
{
    internal sealed class CustomerController
    {
        private readonly Dictionary<string, string> cookies = []; // key - cookie, data - login
        private readonly Dictionary<string, Customer> cookiesByUsers = []; // key - login, data - cookie
        private readonly Dictionary<string, Customer> customers = []; // key - login, data - customer

        private readonly Dictionary<ulong, Customer> bankAccounts = []; // key - bankID, data - bank balance
        private readonly string jsonResponseSuccess;
        private readonly string jsonResponseFailure;

        // 5% investment rate, $5 transaction fee, $1000 overdraft limit
        private readonly decimal newInvestmentRate = 0.05m;
        private readonly decimal newFailedTransactionFee = 5m;
        private readonly decimal newOverdraftLimit = 1000m;

        public CustomerController()
        {
            jsonResponseSuccess = JsonString("result", "success");
            jsonResponseFailure = JsonString("result", "failure");
            LoadAll();
        }

        private void LoadAll()
        {
            if(!Directory.Exists(Terminal.DBPath())) { Directory.CreateDirectory(Terminal.DBPath()); }

            string[] customerFileList = Directory.GetFiles(Terminal.DBPath());
            foreach (var customer in customerFileList)
            {
                if (customer.Contains(".customer"))
                {
                    try
                    {
                        if (LoadBankAccount(false, JsonDocument.Parse(File.ReadAllText(customer)).RootElement.Deserialize<Dictionary<string, string>>()!) == jsonResponseSuccess)
                        {
                            Console.WriteLine("Successfully loaded file: " + customer);
                        }
                    }
                    catch (Exception e) { Terminal.PrintException(e); }
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

        private ulong GenerateBankAccountId()
        {
            ulong bankID = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8));
            while (bankAccounts.ContainsKey(bankID))
            {
                bankID = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8));
            }
            return bankID;
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

        internal string ParseData(Dictionary<string, string> parsedData) // Parse and respond
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
                    else if (requestType == "changeAccountPin")
                    {
                        s = ChangeAccountPin(parsedData);
                    }
                    else if (requestType == "deleteBankAccount")
                    {
                        s = DeleteBankAccount(parsedData);
                    }
                    else if (requestType == "innerTransaction")
                    {
                        s = InnerTransaction(parsedData);
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
            parsedData.TryGetValue("newPassword", out string? newPassword))
            {
                if (customers[cookies[cookie]].ChangePassword(cookie, newPassword))
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

        private string InnerTransaction(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) &&
            parsedData.TryGetValue("cookie", out string? cookie) &&
            parsedData.TryGetValue("pin", out string? pinString) &&
            parsedData.TryGetValue("recipientBankingID", out string? recipientBankingID) &&
            parsedData.TryGetValue("sum", out string? sumString))
            {
                try
                {
                    Customer sender = customers[cookies[cookie]];
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (bankAccounts.TryGetValue(ulong.Parse(recipientBankingID), out Customer recipient))
#pragma warning restore CS8600
                    {
                        if (sender.Transaction(cookie, short.Parse(pinString), "withdraw", Math.Abs(decimal.Parse(sumString))) && 
                            recipient.InnerTransaction(Math.Abs(decimal.Parse(sumString))))
                        {
                            UpdateRecord(DBActionType.Update, sender);
                            UpdateRecord(DBActionType.Update, recipient);
                            return jsonResponseSuccess;
                        }
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string Transaction(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) &&
            parsedData.TryGetValue("cookie", out string? cookie) &&
            parsedData.TryGetValue("pin", out string? pinString) &&
            parsedData.TryGetValue("transactionType", out string? transactionType) &&
            parsedData.TryGetValue("sum", out string? sumString))
            {
                try
                {
                    if (customers[cookies[cookie]].Transaction(cookie,
                    short.Parse(pinString), transactionType, Math.Abs(decimal.Parse(sumString))))
                    {
                        UpdateRecord(DBActionType.Update, customers[cookies[cookie]]);
                        return jsonResponseSuccess;
                    }
                }
                catch (Exception e) { Terminal.PrintException(e); }

            }
            return jsonResponseFailure;
        }

        private string CreateBankAccount(Dictionary<string, string> parsedData)
        {
            return LoadBankAccount(false, parsedData);
        }

        private string LoadBankAccount(bool fromDatabase, Dictionary<string, string> parsedData)
        {
            Console.WriteLine("Loading account");
            foreach (var s in parsedData)
            {
                Console.WriteLine("Key: " + s.Key + "Value: " + s.Value);
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (parsedData.TryGetValue("login", out string login) &&
            parsedData.TryGetValue("password", out string password) &&
            parsedData.TryGetValue("name", out string name) &&
            parsedData.TryGetValue("phone", out string phone) &&
            parsedData.TryGetValue("email", out string email) &&
            parsedData.TryGetValue("accountType", out string accountType) &&
            parsedData.TryGetValue("pin", out string sPin) &&
            parsedData.TryGetValue("isBankStaff", out string sIsBankStaff))
            {
                Console.WriteLine("Creating bank account");
                ulong bankAccountID;
                bool isBankStaff = Convert.ToBoolean(sIsBankStaff);
                decimal balance = 0m;
                decimal investmentRate = newInvestmentRate;
                decimal failedTransactionFee = newFailedTransactionFee;
                decimal overdraftLimit = newOverdraftLimit;
                Customer customer;
                EverydayAccount everydayBalance;
                lock (customers)
                {
                    try
                    {
                        if (fromDatabase && parsedData.TryGetValue("balance", out string sBalance))
                        {
                            bankAccountID = Convert.ToUInt64(parsedData["bankAccountID"]);
                            isBankStaff = Convert.ToBoolean(parsedData["isBankStaff"]);
                            balance = Convert.ToDecimal(sBalance);
                            if (accountType == "Investment" || accountType == "Omni")
                            {
                                if (parsedData.TryGetValue("investmentRate", out string sInvestmentRate) &&
                                    parsedData.TryGetValue("interest", out string sInterest) &&
                                    parsedData.TryGetValue("failedTransactionFee", out string sFailedTransactionFee))
                                {
                                    investmentRate = Convert.ToDecimal(sInvestmentRate);
                                    failedTransactionFee = Convert.ToDecimal(sFailedTransactionFee);
                                    if (parsedData.TryGetValue("overdraftLimit", out string sOverdraftLimit))
                                    {
                                        overdraftLimit = Convert.ToDecimal(sOverdraftLimit);
                                    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                                    //
                                }

                            }
                        }
                        else
                        {
                            bankAccountID = GenerateBankAccountId();
                            if (customers.ContainsKey(login))
                            {
                                return jsonResponseFailure;
                            }
                            Console.WriteLine("ID is generated");
                        }

                        if (accountType == "Everyday")
                        {
                            everydayBalance = new EverydayAccount(isBankStaff, bankAccountID, balance, Convert.ToInt16(sPin));
                        }
                        else if (accountType == "Investment")
                        {
                            everydayBalance = new InvestmentAccount(isBankStaff, bankAccountID, balance, Convert.ToInt16(sPin), investmentRate, failedTransactionFee);
                        }
                        else if (accountType == "Omni")
                        {
                            everydayBalance = new OmniAccount(isBankStaff, bankAccountID, balance, Convert.ToInt16(sPin), investmentRate, failedTransactionFee, overdraftLimit);
                        }
                        else
                        {
                            throw new UndefinedCustomerTypeException();
                        }
                        Console.WriteLine("Bank account is created");
                        customer = new(login, password, name, phone, email, everydayBalance);
                        customers.Add(customer.customerID, customer);
                        bankAccounts.Add(bankAccountID, customer);
                        Console.WriteLine("Customer is created");
                        UpdateRecord(DBActionType.Update, customer);
                        return jsonResponseSuccess;
                    }
                    catch (Exception e) { Terminal.PrintException(e); }
                }
            }
            return jsonResponseFailure;
        }

        private static void UpdateRecord(DBActionType actionType, Customer customer)
        {
            string path = Terminal.DBPath() + Path.DirectorySeparatorChar + customer.customerID + ".customer";
            if (actionType == DBActionType.Update)
            {
                File.WriteAllText(path, customer.Serialize());
            }
            else if (actionType == DBActionType.Delete)
            {
                File.Delete(path);
            }
            Console.WriteLine(path);
        }

        private string ChangeAccountPin(Dictionary<string, string> parsedData)
        {
            if (CookieTest(parsedData) &&
            parsedData.TryGetValue("cookie", out string? cookie) &&
            parsedData.TryGetValue("bankAccountID", out string? bankAccountID) &&
            parsedData.TryGetValue("pin", out string? pinString))
            {
                try
                {
                    if (customers[cookies[cookie]].ChangeAccountPin(cookie,
                    ulong.Parse(bankAccountID), short.Parse(pinString), short.Parse(pinString)))
                    {
                        UpdateRecord(DBActionType.Update, customers[cookies[cookie]]);
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
            parsedData.TryGetValue("cookie", out string? cookie))
            {
                lock (customers)
                {
                    try
                    {
                        string login = cookies[cookie];
                        Customer customer = customers[login];
                        
                        if (customer.GetBalance() != 0) { return jsonResponseFailure; }

                        lock (cookies) { cookies.Remove(cookie); }
                        lock (bankAccounts) { bankAccounts.Remove(customer.GetBankingID()); }
                        lock (cookiesByUsers) { cookiesByUsers.Remove(login); }
                        customers.Remove(login);
                        UpdateRecord(DBActionType.Delete, customer);
                        return jsonResponseSuccess;
                    }
                    catch (Exception e) { Terminal.PrintException(e); }
                }
            }
            return jsonResponseFailure;
        }

        private bool CookieTest(Dictionary<string, string> parsedData) // Simple check if cookie is valid
        {
            if (parsedData.TryGetValue("cookie", out string? cookie)
                && cookies.TryGetValue(parsedData.GetValueOrDefault("cookie")!, out string? login))
            {
                Console.WriteLine(cookie);
                return ValidateSession(cookie, customers[login]);
            }
            return false;
        }
    }
}