using System;

namespace StringMath
{
    public class LangException : Exception
    {
        public LangException(string message) : base(message)
        {
        }
    }
}