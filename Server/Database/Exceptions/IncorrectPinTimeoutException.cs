using System;

namespace Server.Database.Exceptions
{
    class IncorrectPinTimeoutException : Exception
    {
        public IncorrectPinTimeoutException(string message = "Too many incorrect pin login attempts - try later") : base (message) { }
        public IncorrectPinTimeoutException(Exception inner, string message = "Too many incorrect pin login attempts - try later") : base(message, inner) { }
    }
}