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

namespace Server.Database
{
    internal sealed class Customer
    {
        // Password & account ID ----------------------------------------
        internal readonly string customerID;
        private string password;
        private readonly Lock _passwordRWLock = new();

        // Session data ----------------------------------------
        
        internal string SessionID 
        { 
            private set { lock(_sessionRWLock) { _sessionID = value; } } 
            get { lock(_sessionRWLock) { return _sessionID; } }
        }
        private string _sessionID = string.Empty;
        private DateTime sessionCreationTime = DateTime.MinValue;
        private readonly Lock _sessionRWLock = new();
        private readonly Lock _sessionEditRWLock = new();

        // Name ----------------------------------------
        internal string Name 
        { 
            private set { lock(_nameRWLock) { name = value; } } 
            get { lock(_nameRWLock) { return name; } }
        }
        private string  name;
        private readonly Lock _nameRWLock = new();

        // PhoneNumber ----------------------------------------
        internal string PhoneNumber
        {
            private set { lock(_phoneNumberRWLock) { phoneNumber = value; } } 
            get { lock(_phoneNumberRWLock) { return phoneNumber; } }
        }
        private string phoneNumber;
        private readonly Lock _phoneNumberRWLock = new();

        // Email ----------------------------------------
        internal string Email 
        { 
            private set { lock(_emailRWLock) { email = value; } } 
            get { lock(_emailRWLock) { return email; } }
        }
        private string  email;
        private readonly Lock _emailRWLock = new();

        // Other locks ----------------------------------------
        private readonly Lock _contactInfoRWLock = new();
        private readonly Lock _transactionRWLock = new();

        private readonly List<EverydayAccount> accounts;

        // Constructors ----------------------------------------
        public Customer(string newCustomerID, string newPassword, string newName, string newPhone, string newEmail, List<EverydayAccount> newAccounts)
        {
            customerID = newCustomerID;
            password = newPassword;
            name = newName;
            phoneNumber = newPhone;
            email = newEmail;
            accounts = newAccounts;
        }

        // Authorisation ----------------------------------------
        internal bool ValidateSession(string cookie)
        {
            lock (_sessionEditRWLock)
            {
                // If session doesn't exists, 
                // is older than 2 hours 
                // or incorrect 
                // it is considered invalid
                if (string.IsNullOrEmpty(SessionID)
                ||
                DateTime.Now.CompareTo(sessionCreationTime.AddHours(2)) > 0
                ||
                SessionID != cookie)
                { return false; }
                Console.WriteLine("Cookie is verified");
                return true;
            }
        }
        internal bool Login (string newSessionID, string userpassword)
        {
            lock (_passwordRWLock)
            {
                lock (_sessionEditRWLock)
                {
                    if (password == userpassword)
                    {
                        SessionID = newSessionID;
                        sessionCreationTime = DateTime.Now;
                        return true;
                    }
                    return false;
                }
            }
        }
        internal bool Logout(string cookie)
        {
            if (!ValidateSession(cookie)) { return false; }
            lock (_sessionEditRWLock)
            {
                SessionID = string.Empty;
                sessionCreationTime = DateTime.MinValue;
            }
            return true;
        }
        internal bool ChangePassword(string cookie, string newPassword)
        {
            lock (_passwordRWLock)
            {
                if(ValidateSession(cookie)) 
                { 
                    password = newPassword; 
                    return true; 
                }
            }
            return false;
        }

        internal bool EditContactData(string cookie, string newName, string newPhone, string newEmail)
        {
            if (ValidateSession(cookie))
            {
                lock (_contactInfoRWLock)
                {
                    name = newName;
                    phoneNumber = newPhone;
                    email = newEmail;
                }
                return true;
            }
            return false;
        }

        // Bank account instances ----------------------------------------
        internal bool Transaction(string cookie, long bankAccountID, short pin, string transactionType, decimal sum)
        {
            if (ValidateSession(cookie))
            {
                lock (_transactionRWLock)
                {
                    foreach (var account in accounts)
                    {
                        try
                        {
                            if (account.bankID == bankAccountID && account.ValidatePin(pin))
                            {
                                if (transactionType == "deposit") { return account.Deposit(sum); }
                                else if (transactionType == "withdraw") { return account.Withdraw(sum); }
                                else if (transactionType == "addInterest")
                                {
                                    if (account.AccountType == "Investment") { return ((InvestmentAccount)account).AddInterest(); }
                                    else if (account.AccountType == "Omni") { return ((OmniAccount)account).AddInterest(); }
                                }
                                break;
                            }
                        } catch (Exception e) { Terminal.PrintException(e); }
                    }
                }
            }
            return false;
        }

        internal bool CreateBankAccount(string cookie, string accountType, bool valueIsBankStaff, decimal valueBalance, string valueAccountName, short valuePin, decimal newInvestmentRate, decimal newFailedTransactionFee, decimal newOverdraftLimit)
        {
            if (ValidateSession(cookie))
            {
                lock (_transactionRWLock)
                {
                    if(accountType == "Everyday")
                    {
                        accounts.Add(new EverydayAccount(valueIsBankStaff, valueBalance, valueAccountName, valuePin));
                        return true;
                    }
                    else if (accountType == "Investment")
                    {
                        accounts.Add(new InvestmentAccount(valueIsBankStaff, valueBalance, valueAccountName, valuePin, newInvestmentRate, newFailedTransactionFee));
                        return true;
                    }
                    else if (accountType == "Omni")
                    {
                        accounts.Add(new OmniAccount(valueIsBankStaff, valueBalance, valueAccountName, valuePin, newInvestmentRate, newFailedTransactionFee, newOverdraftLimit));
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool EditBankAccount(string cookie, long bankAccountID, string newBankAccountName)
        {
            if (ValidateSession(cookie) && !string.IsNullOrEmpty(newBankAccountName))
            {
                lock (_contactInfoRWLock)
                {
                    foreach (var account in accounts)
                    {
                        if (account.bankID == bankAccountID)
                        {
                            account.AccountName = newBankAccountName;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool ChangeAccountPin(string cookie, long bankAccountID, short currentPin, short newPin)
        {
            if (ValidateSession(cookie) && newPin.ToString().Length == 4)
            {
                lock (_transactionRWLock)
                {
                    foreach (var account in accounts)
                    {
                        if (account.bankID == bankAccountID)
                        {
                            return account.ChangePin(currentPin, newPin);
                        }
                    }
                }
            }
            return false;
        }

        internal bool DeleteBankAccount(string cookie, long bankAccountID, short pin)
        {
            if (ValidateSession(cookie))
            {
                lock (_transactionRWLock)
                {
                    foreach (var account in accounts)
                    {
                        try
                        {
                            if (account.bankID == bankAccountID && account.Balance == 0 && account.ValidatePin(pin))
                            {
                                Console.WriteLine();
                                Console.WriteLine(bankAccountID);
                                accounts.Remove(account);
                                return true;
                            }
                            Console.WriteLine(account.bankID);
                            Console.WriteLine(pin + " " + account.ValidatePin(pin));
                        }
                        catch (Exception e) { Terminal.PrintException(e); }
                    }
                }
            }
            return false;
        }

        // Other ----------------------------------------

        public override string ToString()
        {
            string accountInfo = SVTerminal.JsonString(
                [ ("login", customerID.ToString()), 
                ("name", Name.ToString()), 
                ("phone", PhoneNumber.ToString()), 
                ("email", email.ToString())]);
            
            Dictionary<string, string> accountsDict = [];

            foreach (var account in accounts)
            {
                try
                {
                    accountsDict.Add(account.bankID.ToString(), account.ToString());
                }
                catch (Exception e) { Terminal.PrintException(e); }
            }
            return SVTerminal.JsonString([("accountInfo", accountInfo), ("bankAccounts", SVTerminal.JsonString(accountsDict))]);
        }
    }
}