using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    static class Extensions
    {
        public static T[] RemoveLast<T>(this IEnumerable<T> collection)
        {
            var arr = collection.ToArray();
            var result = new T[arr.Length - 1];
            Array.Copy(arr, 0, result, 0, arr.Length - 1);
            return result;
        }

        public static Token[] GetTokens(this string input, MathContext context)
        {
            var sourceText = new SourceText(input);
            var lexer = new Lexer(sourceText, context);

            var tokens = new List<Token>();
            Token t = lexer.Lex();
            tokens.Add(t);

            while (t.Type != TokenType.EndOfCode)
            {
                t = lexer.Lex();
                tokens.Add(t);
            }

            return tokens.RemoveLast();
        }

        public static IReadOnlyCollection<string> GetReplacements(this string input, MathContext context)
        {
            var sourceText = new SourceText(input);
            var lexer = new Lexer(sourceText, context);
            var parser = new Parser(lexer, context);

            // This populates the replacements collection
            parser.Parse();

            return parser.Replacements;
        }
    }
}