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
    internal class InvestmentBalance : EverydayBalance
    {
        internal override string AccountType { get { return "Investment"; } }
        internal readonly decimal InvestmentRate; // Value between 0-1 for calculating interest

        internal readonly decimal FailedTransactionFee; // A flat fee is applied if transaction failed
        public InvestmentBalance(bool valueIsBankStaff, long valueBankID, decimal valueBalance, short valuePin, decimal newInvestmentRate, decimal newFailedTransactionFee) : base(valueIsBankStaff, valueBankID, valueBalance, valuePin)
        {
            InvestmentRate = newInvestmentRate;
            FailedTransactionFee = newFailedTransactionFee;
            if (valueIsBankStaff){ FailedTransactionFee *= 0.5m; }               
        }

        internal virtual decimal Interest()
        {
            return InvestmentRate * Balance;
        }

        internal bool AddInterest()
        {
            Deposit(Interest());
            return true;
        }

        internal override bool Deposit(decimal value)
        {
            lock (_balanceLock)
            {
                balance = balance + value + value * InvestmentRate;
                Balance = balance;
            }
            return true;
        }
        internal override bool Withdraw(decimal value)
        {
            lock (_balanceLock)
            {
                if (balance >= value)
                {
                    balance -= value;
                    Balance = balance;
                    return true;
                }
                balance -= FailedTransactionFee;
                Balance = balance;
                throw new WithdrawalException();
            }
        }

        public override List<(string, string)> ToList()
        {
            return 
                [ ("bankAccountID", bankID.ToString()), 
                ("isBankStaff", isBankStaff.ToString()), 
                ("accountType", AccountType), 
                ("balance", Balance.ToString()),
                ("investmentRate", InvestmentRate.ToString()),
                ("interest", Interest().ToString()),
                ("failedTransactionFee", FailedTransactionFee.ToString())];
        }
    }
}