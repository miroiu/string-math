namespace StringMath
{
    internal class Parser
    {
        private readonly Lexer _lexer;

        public Token Current { get; private set; }

        public Parser(Lexer lexer)
            => _lexer = lexer;

        public Expression Parse()
        {
            Current = _lexer.Lex();
            return ParseBinaryExpression();
        }

        private Expression ParseBinaryExpression(Expression left = default, OperatorPrecedence parentPrecedence = OperatorPrecedence.None)
        {
            if (left == default)
            {
                if (IsUnaryOperator(Current.Text))
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
                if (IsBinaryOperator(Current.Text))
                {
                    var precedence = GetBinaryOperatorPrecedence(Current);
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

        public static OperatorPrecedence GetBinaryOperatorPrecedence(Token token)
        {
            switch (token.Text)
            {
                case "+":
                case "-":
                    return OperatorPrecedence.Addition;

                case "*":
                case "/":
                    return OperatorPrecedence.Multiplication;

                case "^":
                    return OperatorPrecedence.Power;
            }

            throw new LangException($"'{token.Type}' is not an operator.");
        }

        public static bool IsUnaryOperator(string type)
        {
            switch (type)
            {
                case "!":
                    return true;

                case "-":
                    return true;
            }

            return false;
        }

        public static bool IsBinaryOperator(string type)
        {
            switch (type)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                    return true;
            }

            return false;
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

        public void Ensure(TokenType type)
        {
            if (Current.Type != type)
            {
                throw new LangException($"Expected {type} but found {Current.Type}.");
            }
        }

        public bool IsEndOfStatement()
            => Current.Type == TokenType.EndOfCode;
    }
}
