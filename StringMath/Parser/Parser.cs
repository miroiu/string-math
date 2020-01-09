namespace StringMath
{
    internal class Parser
    {
        private readonly Lexer _lexer;
        private readonly MathContext _mathContext;

        public Token Current { get; private set; }

        public Parser(Lexer lexer, MathContext mathContext)
        {
            _lexer = lexer;
            _mathContext = mathContext;
        }

        public Expression Parse()
        {
            Current = _lexer.Lex();
            return ParseBinaryExpression();
        }

        private Expression ParseBinaryExpression(Expression left = default, OperatorPrecedence parentPrecedence = OperatorPrecedence.None)
        {
            if (left == default)
            {
                if (_mathContext.IsUnaryOperator(Current.Text))
                {
                    var operatorToken = Take();
                    left = new UnaryExpression(operatorToken.Text, ParseBinaryExpression(left, OperatorPrecedence.Prefix));
                }
                else
                {
                    left = ParsePrimaryExpression();

                    if (Current.Type == TokenType.Exclamation)
                    {
                        var operatorToken = Take();
                        left = new UnaryExpression(operatorToken.Text, left);
                    }
                }
            }

            while (!IsEndOfStatement())
            {
                if (_mathContext.IsBinaryOperator(Current.Text))
                {
                    var precedence = _mathContext.GetBinaryOperatorPrecedence(Current.Text);
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
            switch (Current.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take().Text);

                case TokenType.Identifier:
                    return new ReplacementExpression(Take().Text);

                case TokenType.OpenParen:
                    return ParseGroupingExpression();
            }

            throw new LangException($"Unexpected token '{Current.Text}'.");
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
            if (Current.Type == tokenType)
            {
                return Take();
            }

            throw new LangException($"Expected {tokenType} but found {Current.Type}.");
        }

        public Token Take()
        {
            var previous = Current;
            Current = _lexer.Lex();
            return previous;
        }

        public bool IsEndOfStatement()
            => Current.Type == TokenType.EndOfCode;
    }
}
