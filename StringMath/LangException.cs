using System;

namespace StringMath
{
    /// <summary>Base library exception.</summary>
    public class LangException : Exception
    {
        /// <summary>Available error codes.</summary>
        public enum ErrorCode
        {
            /// <summary>Unexpected token.</summary>
            UNEXPECTED_TOKEN = 0,
            /// <summary>Unassigned variable.</summary>
            UNASSIGNED_VARIABLE = 1
        }

        /// <summary>Initializes a new instance of a <see cref="LangException"/>.</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message to describe the error.</param>
        internal LangException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorType = errorCode;
        }

        /// <summary>Initializes a new instance of a <see cref="LangException"/>.</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message to describe the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        internal LangException(ErrorCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorType = errorCode;
        }

        /// <summary>The error code of the exception.</summary>
        public ErrorCode ErrorType { get; }

        /// <summary>The position of the token where the exception was raised.</summary>
        public int Position { get; private set; }

        internal static LangException UnexpectedToken(Token token, string? expected = default)
        {
            string expectedMessage = expected != default ? $" Expected {expected}" : string.Empty;
            return new LangException(ErrorCode.UNEXPECTED_TOKEN, $"Unexpected token `{token.Text}` at position {token.Position}.{expectedMessage}")
            {
                Position = token.Position
            };
        }

        internal static LangException UnexpectedToken(Token token, char expected)
        {
            return UnexpectedToken(token, expected.ToString());
        }

        internal static LangException UnexpectedToken(Token token, TokenType tokenType)
        {
            return UnexpectedToken(token, tokenType.ToReadableString());
        }

        internal static Exception UnassignedVariable(VariableExpression variableExpr)
        {
            return new LangException(ErrorCode.UNASSIGNED_VARIABLE, $"Use of unassigned variable `{variableExpr.Name}`.");
        }
    }
}