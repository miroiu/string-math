using System;

namespace SCLang
{
    public class LangException : Exception
    {
        public LangException(string message) : base(message)
        {
        }
    }
}