using System.Collections.Generic;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static List<Token> ReadAllTokens(this string input)
        {
            ITokenizer tokenizer = new Tokenizer(input);
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