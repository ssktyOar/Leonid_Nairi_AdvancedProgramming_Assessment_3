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
            private set { lock (_sessionRWLock) { _sessionID = value; } }
            get { lock (_sessionRWLock) { return _sessionID; } }
        }
        private string _sessionID = string.Empty;
        private DateTime sessionCreationTime = DateTime.MinValue;
        private readonly Lock _sessionRWLock = new();
        private readonly Lock _sessionEditRWLock = new();

        // Name ----------------------------------------
        internal string Name
        {
            private set { lock (_nameRWLock) { name = value; } }
            get { lock (_nameRWLock) { return name; } }
        }
        private string name;
        private readonly Lock _nameRWLock = new();

        // PhoneNumber ----------------------------------------
        internal string PhoneNumber
        {
            private set { lock (_phoneNumberRWLock) { phoneNumber = value; } }
            get { lock (_phoneNumberRWLock) { return phoneNumber; } }
        }
        private string phoneNumber;
        private readonly Lock _phoneNumberRWLock = new();

        // Email ----------------------------------------
        internal string Email
        {
            private set { lock (_emailRWLock) { email = value; } }
            get { lock (_emailRWLock) { return email; } }
        }
        private string email;
        private readonly Lock _emailRWLock = new();

        // Other locks ----------------------------------------
        private readonly Lock _contactInfoRWLock = new();
        private readonly Lock _transactionRWLock = new();

        private readonly EverydayBalance account;

        // Constructors ----------------------------------------
        public Customer(string newCustomerID, string newPassword, string newName, string newPhone, string newEmail, EverydayBalance newAccount)
        {
            customerID = newCustomerID;
            password = newPassword;
            name = newName;
            phoneNumber = newPhone;
            email = newEmail;
            account = newAccount;
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
        internal bool Login(string newSessionID, string userpassword)
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
                if (ValidateSession(cookie))
                {
                    lock (password)
                    {
                        password = newPassword;
                    }
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
        internal bool Transaction(string cookie, short pin, string transactionType, decimal sum)
        {
            if (ValidateSession(cookie))
            {
                lock (_transactionRWLock)
                {
                    try
                    {
                        if (account.ValidatePin(pin))
                        {
                            if (transactionType == "deposit") { return account.Deposit(sum); }
                            else if (transactionType == "withdraw") { return account.Withdraw(sum); }
                            else if (transactionType == "addInterest")
                            {
                                if (account.AccountType == "Investment") { return ((InvestmentBalance)account).AddInterest(); }
                                else if (account.AccountType == "Omni") { return ((OmniBalance)account).AddInterest(); }
                            }
                        }
                    }
                    catch (Exception e) { Terminal.PrintException(e); }
                }
            }
            return false;
        }

        internal bool InnerTransaction(decimal sum)
        {
                lock (_transactionRWLock)
                {
                    try
                    {
                        return account.Deposit(sum);
                    }
                    catch (Exception e) { Terminal.PrintException(e); }
                }
            return false;
        }

        internal bool EditBankAccount(string cookie, string newBankAccountName)
        {
            if (ValidateSession(cookie) && !string.IsNullOrEmpty(newBankAccountName))
            {
                lock (_contactInfoRWLock)
                {
                    account.AccountName = newBankAccountName;
                    return true;
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
                    if (account.bankID == bankAccountID)
                    {
                        return account.ChangePin(currentPin, newPin);
                    }
                }
            }
            return false;
        }

        // Other ----------------------------------------

        internal long GetBankingID() { return account.bankID; }

        public override string ToString()
        {
            List<(string, string)> accountInfo = 
                [ ("login", customerID.ToString()),
                ("name", Name.ToString()),
                ("phone", PhoneNumber.ToString()),
                ("email", email.ToString())];
            accountInfo.AddRange(account.ToList());
            return SVTerminal.JsonString(accountInfo.ToArray());
        }
    }
}