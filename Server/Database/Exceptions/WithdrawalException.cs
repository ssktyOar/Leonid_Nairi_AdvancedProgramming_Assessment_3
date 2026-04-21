using System;

namespace Server.Database.Exceptions
{
    class WithdrawalException : Exception
    {
        public WithdrawalException(string message = "Insufficient funds: Cannot withdraw") : base (message) { }
        public WithdrawalException(Exception inner, string message = "Insufficient funds: Cannot withdraw") : base(message, inner) { }
    }
}