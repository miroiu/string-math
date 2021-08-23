using System;

namespace StringMath
{
    public class LangException : Exception
    {
        public LangException(string message) : base(message)
        {
        }

        public LangException()
        {
        }

        public LangException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}