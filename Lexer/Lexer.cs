namespace SCLang
{
    internal partial class Lexer
    {
        private readonly SourceText _text;

        public Lexer(SourceText text)
            => _text = text;

        public Token Lex()
        {
            Token token = new Token
            {
                Text = $"{_text.Current}",
                Position = _text.Position
            };

            switch (_text.Current)
            {
                case '\0':
                    token.Type = TokenType.EndOfCode;
                    break;

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

                case '+':
                    token.Type = TokenType.Plus;
                    _text.MoveNext();
                    break;

                case '-':
                    token.Type = TokenType.Minus;
                    _text.MoveNext();
                    break;

                case '/':
                    token.Type = TokenType.Slash;
                    _text.MoveNext();
                    break;

                case '*':
                    token.Type = TokenType.Asterisk;
                    _text.MoveNext();
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
                    token.Type = TokenType.OpenCurly;
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

                default:
                    throw new LangException($"Unexpected character {_text.Current}");
            }

            return token;
        }
    }
}
