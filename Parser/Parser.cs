using System.Collections.Generic;

namespace SCLang
{
    internal class Parser
    {
        private readonly Lexer _lexer;

        public Token Current { get; private set; }

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
        }

        public Expression Parse()
            => ParseBinaryExpression();

        private Expression ParseBinaryExpression(Expression left = default, OperatorPrecedence parentPrecedence = OperatorPrecedence.None)
        {
            if (left == default)
            {
                // No need for unary parslet
                if (IsUnaryOperator(Current.Type))
                {
                    var operatorToken = Take();
                    var operatorType = GetUnaryOperatorType(operatorToken);

                    left = new UnaryExpression(operatorType, ParseBinaryExpression(left, OperatorPrecedence.Prefix));
                }
                else
                {
                    left = ParsePrimaryExpression();
                }
            }

            while (!IsEndOfStatement())
            {
                if (IsBinaryOperator(Current.Type))
                {
                    var precedence = GetBinaryOperatorPrecedence(Current);
                    if (parentPrecedence >= precedence)
                    {
                        return left;
                    }

                    var operatorToken = Take();
                    var operatorType = GetBinaryOperatorType(operatorToken);
                    left = new BinaryExpression(left, operatorType, ParseBinaryExpression(parentPrecedence: precedence));
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
                    return new NumberExpression(Current.Text);

                case TokenType.Identifier:
                    return new ReplacementExpression(Current.Text);
            }

            throw new LangException($"Unexpected token '{Current.Text}'.");
        }

        public static OperatorPrecedence GetBinaryOperatorPrecedence(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return OperatorPrecedence.Addition;

                case TokenType.Asterisk:
                case TokenType.Slash:
                    return OperatorPrecedence.Multiplication;
            }

            throw new LangException($"'{token.Type}' is not an operator.");
        }

        public static string GetUnaryOperatorType(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Exclamation:
                    return "!";

                case TokenType.Minus:
                    return "-";
            }

            throw new LangException($"'{token.Type}' is not an unary operator.");
        }

        public static string GetBinaryOperatorType(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                    return "+";

                case TokenType.Minus:
                    return "-";

                case TokenType.Asterisk:
                    return "*";

                case TokenType.Slash:
                    return "/";
            }

            throw new LangException($"{token.Type} is not a binary operator.");
        }

        public static bool IsUnaryOperator(TokenType type)
        {
            switch (type)
            {
                case TokenType.Exclamation:
                    return true;

                case TokenType.Minus:
                    return true;
            }

            return false;
        }

        public static bool IsBinaryOperator(TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Asterisk:
                case TokenType.Slash:
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
