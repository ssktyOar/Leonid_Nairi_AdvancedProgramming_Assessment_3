using System;

namespace Server.Database.Exceptions
{
    class IncorrectPinException : Exception
    {
        public IncorrectPinException(string message = "Incorrect pin: access denied") : base (message) { }
        public IncorrectPinException(Exception inner, string message = "Incorrect pin: access denied") : base(message, inner) { }
    }
}