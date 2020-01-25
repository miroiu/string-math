namespace StringMath
{
    internal sealed class Parser
    {
        private readonly Lexer _lexer;
        private readonly MathContext _mathContext;
        private Token _currentToken;

        public Parser(Lexer lexer, MathContext mathContext)
        {
            _lexer = lexer;
            _mathContext = mathContext;
        }

        public Expression Parse()
        {
            _currentToken = _lexer.Lex();
            return ParseBinaryExpression();
        }

        private Expression ParseBinaryExpression(Expression left = default, OperatorPrecedence parentPrecedence = OperatorPrecedence.None)
        {
            if (left == default)
            {
                if (_mathContext.IsUnaryOperator(_currentToken.Text))
                {
                    var operatorToken = Take();
                    left = new UnaryExpression(operatorToken.Text, ParseBinaryExpression(left, OperatorPrecedence.Prefix));
                }
                else
                {
                    left = ParsePrimaryExpression();

                    if (_currentToken.Type == TokenType.Exclamation)
                    {
                        var operatorToken = Take();
                        left = new UnaryExpression(operatorToken.Text, left);
                    }
                }
            }

            while (!IsEndOfStatement())
            {
                if (_mathContext.IsBinaryOperator(_currentToken.Text))
                {
                    var precedence = _mathContext.GetBinaryOperatorPrecedence(_currentToken.Text);
                    if (parentPrecedence >= precedence)
                    {
                        return left;
                    }

                    var operatorToken = Take();
                    left = new BinaryExpression(left, operatorToken.Text, ParseBinaryExpression(parentPrecedence: precedence));
                }
                else
                {
                    return left;
                }
            }

            return left;
        }

        public Expression ParsePrimaryExpression()
        {
            switch (_currentToken.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take().Text);

                case TokenType.Identifier:
                    return new ReplacementExpression(Take().Text);

                case TokenType.OpenParen:
                    return ParseGroupingExpression();
            }

            throw new LangException($"Unexpected token '{_currentToken.Text}'.");
        }

        private Expression ParseGroupingExpression()
        {
            Take();

            var expr = ParseBinaryExpression();

            Match(TokenType.CloseParen);

            return new GroupingExpression(expr);
        }

        public Token Match(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
            {
                return Take();
            }

            throw new LangException($"Expected '{tokenType}' but found '{_currentToken.Type}'.");
        }

        public Token Take()
        {
            var previous = _currentToken;
            _currentToken = _lexer.Lex();
            return previous;
        }

        public bool IsEndOfStatement()
            => _currentToken.Type == TokenType.EndOfCode;
    }
}
