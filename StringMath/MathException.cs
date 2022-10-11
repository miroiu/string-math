using System;
using StringMath.Expressions;

namespace StringMath
{
    /// <summary>Base library exception.</summary>
    public sealed class MathException : Exception
    {
        /// <summary>Available error codes.</summary>
        public enum ErrorCode
        {
            /// <summary>Unexpected token.</summary>
            UNEXPECTED_TOKEN = 0,
            /// <summary>Unassigned variable.</summary>
            UNASSIGNED_VARIABLE = 1,
            /// <summary>Missing operator.</summary>
            UNDEFINED_OPERATOR = 2,
            /// <summary>Missing variable.</summary>
            UNEXISTING_VARIABLE = 4,
            /// <summary>Readonly variable.</summary>
            READONLY_VARIABLE = 8,
        }

        /// <summary>Initializes a new instance of a <see cref="MathException"/>.</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message to describe the error.</param>
        private MathException(ErrorCode errorCode, string message) : base(message)
        {
            Code = errorCode;
        }

        /// <summary>The error code of the exception.</summary>
        public ErrorCode Code { get; }

        /// <summary>The position of the token where the exception was raised.</summary>
        public int Position { get; private set; }

        internal static MathException UnexpectedToken(Token token, string? expected = default)
        {
            string expectedMessage = expected != default ? $" Expected {expected}" : string.Empty;
            return new MathException(ErrorCode.UNEXPECTED_TOKEN, $"Unexpected token `{token.Text}` at position {token.Position}.{expectedMessage}")
            {
                Position = token.Position
            };
        }

        internal static MathException UnexpectedToken(Token token, char expected)
            => UnexpectedToken(token, expected.ToString());

        internal static MathException UnexpectedToken(Token token, TokenType tokenType)
            => UnexpectedToken(token, tokenType.ToReadableString());

        internal static Exception UnassignedVariable(VariableExpression variableExpr)
            => new MathException(ErrorCode.UNASSIGNED_VARIABLE, $"Use of unassigned variable '{variableExpr.Name}'.");

        internal static Exception MissingBinaryOperator(string op)
            => new MathException(ErrorCode.UNDEFINED_OPERATOR, $"Undefined binary operator '{op}'.");
       
        internal static Exception MissingUnaryOperator(string op)
            => new MathException(ErrorCode.UNDEFINED_OPERATOR, $"Undefined unary operator '{op}'.");

        internal static Exception MissingVariable(string variable)
            => new MathException(ErrorCode.UNEXISTING_VARIABLE, $"Variable '{variable}' does not exist.");

        internal static Exception ReadonlyVariable(string name)
            => new MathException(ErrorCode.READONLY_VARIABLE, $"Variable '{name}' is read-only.");
    }
}