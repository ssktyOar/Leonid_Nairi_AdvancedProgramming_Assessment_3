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
    internal class EverydayBalance
    {
        internal static readonly object _idGenLock = new();
        internal readonly ulong bankID;
        internal readonly bool isBankStaff;
        internal virtual string AccountType { get { return "Everyday"; } }

        // Balance ----------------------------------------

        internal decimal Balance // Refactored using new C# 14 feature - "field" keyword
        {
            // Still have to use lock objects because decimal is a struct
            private protected set { lock(_balanceRWLock) { field = value; } } // Thread-safe reading and writing
            get { lock(_balanceRWLock) { return field; } } 
        }
        private protected decimal balance;
        // Can't use System.Threading.Lock on the structs,
        // have to lock default object instances instead
        private protected readonly object _balanceRWLock = new();        
        private protected readonly object _balanceLock = new();
        private protected readonly object _accountNameRWLock = new();

        // Pin ----------------------------------------
        private short pin;
        private readonly Lock _pinRWLock = new();
        private DateTime lastFailedAttempt = DateTime.MinValue;
        private byte count = 0;

        // Constructors ----------------------------------------

        public EverydayBalance(bool valueIsBankStaff, ulong valueBankID, decimal valueBalance, short valuePin)
        {
            isBankStaff = valueIsBankStaff;
            bankID = valueBankID;
            balance = valueBalance;
            Balance = valueBalance;
            pin = valuePin;
        }

        // Methods ----------------------------------------
        
        internal bool ValidatePin(short inputPin)
        {
            lock (_pinRWLock)
            {
                // Protection is set up to protect from bad actor bruteforcing pin
                // If no mistakes made in a 5 hour span, counter resets
                // If a mistake made within 5 hour span, counter adds up
                // If less than 5 hours passed since making 3 mistakes, action is forbidden...
                // ...but timer is not renewed
                if (count >= 3 && DateTime.Now.CompareTo(lastFailedAttempt.AddHours(5)) < 0)
                {
                    throw new IncorrectPinTimeoutException();
                }
                count = 0;
                if (inputPin == pin)
                {
                    count = 0;
                    Console.WriteLine("Pin is verified");
                    return true;
                }
                Console.WriteLine("Incorrect pin: " + inputPin.ToString());
                Console.WriteLine("Expected: " + pin);
                lastFailedAttempt = DateTime.Now;
                count++;
                throw new IncorrectPinException();
            }
        }
        internal bool ChangePin(short currentPin, short newPin)
        {
            lock (_pinRWLock)
            {
                if (ValidatePin(currentPin))
                {
                    pin = newPin;
                    return true;
                }
                return false;                
            }
        }
        
        internal short GetPin()
        {
            lock (_pinRWLock)
            {
                return pin;
            }
        }

        internal virtual bool Deposit(decimal value)
        {
            lock (_balanceLock)
            {
                balance += value;
                Balance = balance;
            }
            return true;
        }
        internal virtual bool Withdraw(decimal value)
        {
            lock (_balanceLock)
            {
                if (balance >= value)
                {
                    balance -= value;
                    Balance = balance;
                    return true;
                }

                throw new WithdrawalException ();
            }
        }

        public virtual List<(string, string)> ToList()
        {
            return 
                [ ("bankAccountID", bankID.ToString()), 
                ("isBankStaff", isBankStaff.ToString()), 
                ("accountType", AccountType), 
                ("balance", Balance.ToString())];
        }
    }
}