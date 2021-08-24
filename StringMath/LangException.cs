using System;

namespace StringMath
{
    /// <summary>Base library exception.</summary>
    public class LangException : Exception
    {
        /// <summary>Initializes a new instance of a <see cref="LangException"/>.</summary>
        /// <param name="message">The message to describe the error.</param>
        public LangException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of a <see cref="LangException"/>.</summary>
        public LangException()
        {
        }

        /// <summary>Initializes a new instance of a <see cref="LangException"/>.</summary>
        /// <param name="message">The message to describe the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public LangException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}