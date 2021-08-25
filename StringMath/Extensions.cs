using System;

namespace StringMath
{
    internal static class Extensions
    {
        public static void EnsureNotNull<T>(this T value, string name) where T : class
        {
            if (value == default)
            {
                throw new ArgumentNullException(name);
            }
        }

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
    }
}
