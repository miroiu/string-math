using System;
using System.Globalization;

namespace StringMath
{
    /// <summary>Helpful extension methods.</summary>
    internal static class Extensions
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is null.</summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="value">The parameter's value.</param>
        /// <param name="name">The parameter's name.</param>
        public static void EnsureNotNull<T>(this T value, string name) where T : class
        {
            if (value == default)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>Converts a token type to a readable string.</summary>
        /// <param name="tokenType">The token type.</param>
        /// <returns>A readable string.</returns>
        public static string ToReadableString(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Identifier => "identifier",
                TokenType.Number => "number",
                TokenType.Operator => "operator",
                TokenType.EndOfCode => "[EOC]",
                TokenType.OpenParen => "(",
                TokenType.CloseParen => ")",
                TokenType.Exclamation => "!",
                _ => tokenType.ToString(),
            };
        }

        /// <summary>Converts a constant expression to a value expression.</summary>
        /// <param name="constantExpr">A constant expression.</param>
        /// <returns>A value expression.</returns>
        public static ValueExpression ToValueExpression(this ConstantExpression constantExpr)
        {
            double value = double.Parse(constantExpr.Value, CultureInfo.InvariantCulture.NumberFormat);
            return new ValueExpression(value);
        }
    }
}
