using StringMath.Expressions;

namespace StringMath
{
    /// <summary>A simple parser.</summary>
    internal sealed class Parser : IParser
    {
        private readonly ITokenizer _tokenzier;
        private readonly IMathContext _mathContext;
        private Token _currentToken;

        /// <summary>Initializes a new instance of a parser.</summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="mathContext">The math context.</param>
        public Parser(ITokenizer tokenizer, IMathContext mathContext)
        {
            _tokenzier = tokenizer;
            _mathContext = mathContext;
        }

        /// <inheritdoc />
        public IExpression Parse()
        {
            _currentToken = _tokenzier.ReadToken();
            IExpression result = ParseBinaryExpression();
            Match(TokenType.EndOfCode);
            return result;
        }

        private IExpression ParseBinaryExpression(IExpression? left = default, Precedence? parentPrecedence = default)
        {
            if (left == default)
            {
                if (_mathContext.IsUnary(_currentToken.Text))
                {
                    Token operatorToken = Take();
                    left = new UnaryExpression(operatorToken.Text, ParseBinaryExpression(left, Precedence.Prefix));
                }
                else
                {
                    left = ParsePrimaryExpression();

                    if (_currentToken.Type == TokenType.Exclamation)
                    {
                        Token operatorToken = Take();
                        left = new UnaryExpression(operatorToken.Text, left);
                    }
                }
            }

            while (!IsEndOfStatement())
            {
                if (_mathContext.IsBinary(_currentToken.Text))
                {
                    Precedence precedence = _mathContext.GetBinaryPrecedence(_currentToken.Text);
                    if (parentPrecedence >= precedence)
                    {
                        return left;
                    }

                    Token operatorToken = Take();
                    left = new BinaryExpression(left, operatorToken.Text, ParseBinaryExpression(parentPrecedence: precedence));
                }
                else
                {
                    return left;
                }
            }

            return left;
        }

        private IExpression ParsePrimaryExpression()
        {
            switch (_currentToken.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take().Text);

                case TokenType.Identifier:
                    string text = Take().Text;
                    string name = text.Substring(1, text.Length - 2);
                    return new VariableExpression(name);

                case TokenType.OpenParen:
                    return ParseGroupingExpression();

                default:
                    throw MathException.UnexpectedToken(_currentToken);
            }
        }

        private IExpression ParseGroupingExpression()
        {
            Take();

            IExpression expr = ParseBinaryExpression();
            Match(TokenType.CloseParen);

            return expr;
        }

        private Token Match(TokenType tokenType)
        {
            return _currentToken.Type == tokenType
                ? Take()
                : throw MathException.UnexpectedToken(_currentToken, tokenType);
        }

        private Token Take()
        {
            Token previous = _currentToken;
            _currentToken = _tokenzier.ReadToken();
            return previous;
        }

        private bool IsEndOfStatement()
        {
            return _currentToken.Type == TokenType.EndOfCode;
        }
    }
}