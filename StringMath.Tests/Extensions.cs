using System.Collections.Generic;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static List<Token> ReadAllTokens(this string input, IMathContext context)
        {
            Tokenizer tokenizer = new Tokenizer(input, context);
            List<Token> tokens = new List<Token>();

            Token t;
            do
            {
                t = tokenizer.ReadToken();
                tokens.Add(t);
            }
            while (t.Type != TokenType.EndOfCode);

            return tokens;
        }
    }
}