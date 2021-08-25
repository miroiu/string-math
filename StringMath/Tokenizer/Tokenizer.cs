using System.Collections.Generic;

namespace StringMath
{
    internal interface ITokenizer
    {
        Token ReadToken();
    }

    internal sealed partial class Tokenizer : ITokenizer
    {
        private readonly ISourceText _text;

        // Excluded characters for custom operators
        private readonly HashSet<char> _invalidOperatorCharacters = new HashSet<char>
        {
            '(', ')', '{', '}', '!', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\0'
        };

        public Tokenizer(ISourceText text)
        {
            _text = text;
        }

        public Tokenizer(string input) : this(new SourceText(input))
        {
        }

        public Token ReadToken()
        {
            Token token = new Token
            {
                Text = $"{_text.Current}",
                Position = _text.Position
            };

            switch (_text.Current)
            {
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    token.Type = TokenType.Number;
                    token.Text = ReadNumber(_text);
                    break;

                case '(':
                    token.Type = TokenType.OpenParen;
                    _text.MoveNext();
                    break;

                case ')':
                    token.Type = TokenType.CloseParen;
                    _text.MoveNext();
                    break;

                case '{':
                    token.Type = TokenType.Identifier;
                    token.Text = ReadIdentifier(_text);
                    break;

                case '!':
                    token.Type = TokenType.Exclamation;
                    _text.MoveNext();
                    break;

                case ' ':
                case '\t':
                    ReadWhiteSpace(_text);
                    return ReadToken();

                case '\r':
                case '\n':
                    _text.MoveNext();
                    return ReadToken();

                case '\0':
                    token.Type = TokenType.EndOfCode;
                    break;

                default:
                    string op = ReadOperator(_text);
                    token.Text = op;
                    token.Type = TokenType.Operator;
                    break;
            }

            return token;
        }

        public override string ToString()
        {
            return _text.ToString();
        }
    }
}
