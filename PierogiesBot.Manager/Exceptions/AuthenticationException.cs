using System;

namespace PierogiesBot.Manager.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
            
        }
    }
}