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

        private Expression ParseBinaryExpression(Expression left = default, Precedence parentPrecedence = default)
        {
            if (left == default)
            {
                if (_mathContext.IsUnaryOperator(_currentToken.Text))
                {
                    var operatorToken = Take();
                    left = new UnaryExpression(operatorToken.Text, ParseBinaryExpression(left, Precedence.Prefix));
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

            throw new LangException($"Expected '{GetString(tokenType)}' but found '{_currentToken.Text}'.");
        }

        public Token Take()
        {
            var previous = _currentToken;
            _currentToken = _lexer.Lex();
            return previous;
        }

        public bool IsEndOfStatement()
            => _currentToken.Type == TokenType.EndOfCode;

        private string GetString(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Identifier:
                    return "variable";

                case TokenType.Number:
                    return "number";

                case TokenType.Operator:
                    return "operator";

                case TokenType.EndOfCode:
                    return "\0";

                case TokenType.OpenParen:
                    return "(";

                case TokenType.CloseParen:
                    return ")";

                case TokenType.Exclamation:
                    return "!";
            }

            return tokenType.ToString();
        }
    }
}