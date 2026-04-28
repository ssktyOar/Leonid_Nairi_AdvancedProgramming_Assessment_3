using System;

namespace Server.Database.Exceptions
{
    class UndefinedCustomerTypeException : Exception
    {
        public UndefinedCustomerTypeException(string message = "Customer type is not recognised") : base (message) { }
        public UndefinedCustomerTypeException(Exception inner, string message = "Customer type is not recognised") : base(message, inner) { }
    }
}