namespace StringMath
{
    internal sealed partial class Lexer
    {
        private readonly SourceText _text;
        private readonly MathContext _mathContext;

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
                    if (_mathContext.IsOperator(_text.Current.ToString()))
                    {
                        token.Type = TokenType.Operator;
                        _text.MoveNext();
                    }
                    else if (char.IsLetter(_text.Current))
                    {
                        string operatorName = ReadOperatorName(_text);
                        token.Type = TokenType.Operator;
                        token.Text = operatorName;
                    }
                    else
                    {
                        throw new LangException($"Unexpected character '{_text.Current}'");
                    }
                    break;
            }

            return token;
        }
    }
}
