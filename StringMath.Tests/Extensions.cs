using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static T[] RemoveLast<T>(this IEnumerable<T> collection)
        {
            T[] arr = collection.ToArray();
            T[] result = new T[arr.Length - 1];
            Array.Copy(arr, 0, result, 0, arr.Length - 1);
            return result;
        }

        public static Token[] GetTokens(this string input, MathContext context)
        {
            SourceText sourceText = new SourceText(input);
            Lexer lexer = new Lexer(sourceText, context);

            List<Token> tokens = new List<Token>();
            Token t = lexer.Lex();
            tokens.Add(t);

            while (t.Type != TokenType.EndOfCode)
            {
                t = lexer.Lex();
                tokens.Add(t);
            }

            return tokens.RemoveLast();
        }

        public static IReadOnlyCollection<string> GetVariables(this string input, MathContext context)
        {
            SourceText sourceText = new SourceText(input);
            Lexer lexer = new Lexer(sourceText, context);
            Parser parser = new Parser(lexer, context);

            // This populates the variables collection
            parser.Parse();

            return parser.Variables;
        }
    }
}