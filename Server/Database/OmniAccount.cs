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
    internal class OmniAccount : InvestmentAccount
    {
        internal override string AccountType { get { return "Omni"; } }
        internal readonly decimal OverdraftLimit;
        public OmniAccount(bool valueIsBankStaff, ulong valueBankID, decimal valueBalance, short valuePin, decimal newInvestmentRate, decimal newFailedTransactionFee, decimal newOverdraftLimit) : base(valueIsBankStaff, valueBankID, valueBalance, valuePin, newInvestmentRate, newFailedTransactionFee)
        {             
            OverdraftLimit = newOverdraftLimit;
        }

        internal override decimal Interest()
        {
            if (Balance < 1000m){ return 0m; }
            return InvestmentRate * Balance;
        }

        internal override bool Deposit(decimal value)
        {
            lock (_balanceLock)
            {
                balance += value;
                Balance = balance;
            }
            return true;
        }
        internal override bool Withdraw(decimal value)
        {
            lock (_balanceLock)
            {
                Console.WriteLine("Results: " + (balance - value + OverdraftLimit).ToString());
                if (balance - value + OverdraftLimit >= 0)
                {
                    balance -=value;
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
                ("failedTransactionFee", FailedTransactionFee.ToString()),
                ("overdraftLimit", OverdraftLimit.ToString())];
        }
    }
}