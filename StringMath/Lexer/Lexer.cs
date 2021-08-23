using System.Collections.Generic;

namespace StringMath
{
    internal sealed partial class Lexer
    {
        private readonly SourceText _text;
        private readonly MathContext _mathContext;

        // Can not create custom operators using these characters
        private readonly HashSet<char> _invalidOperatorCharacters = new HashSet<char>
        {
            '(', ')', '{', '}', '!', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\t', '\r', '\n', '\0'
        };

        public Lexer(SourceText text, MathContext mathContext)
        {
            _text = text;
            _mathContext = mathContext;
        }

        public Token Lex()
        {
            Token token = new Token
            {
                Text = $"{_text.Current}",
                Position = _text.Position
            };

            switch (_text.Current)
            {
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
                    return Lex();

                case '\r':
                case '\n':
                    _text.MoveNext();
                    return Lex();

                case '\0':
                    token.Type = TokenType.EndOfCode;
                    break;

                default:
                    if (char.IsLetter(_text.Current))
                    {
                        string operatorName = ReadOperatorName(_text);
                        token.Type = TokenType.Operator;
                        token.Text = operatorName;
                    }
                    else
                    {
                        string op = ReadOperator(_text);
                        if (_mathContext.IsOperator(op))
                        {
                            token.Text = op;
                            token.Type = TokenType.Operator;
                        }
                        else
                        {
                            throw new LangException($"'{op}' is not an operator.");
                        }
                    }
                    break;
            }

            return token;
        }
    }
}
