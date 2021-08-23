using System.Collections.Generic;

namespace StringMath
{
    internal sealed class Parser
    {
        private readonly Lexer _lexer;
        private readonly MathContext _mathContext;
        private Token _currentToken;
        private readonly HashSet<string> _replacements = new HashSet<string>();

        public IReadOnlyCollection<string> Replacements => _replacements;

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
                if (_mathContext.IsBinaryOperator(_currentToken.Text))
                {
                    Precedence precedence = _mathContext.GetBinaryOperatorPrecedence(_currentToken.Text);
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

        public Expression ParsePrimaryExpression()
        {
            switch (_currentToken.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take().Text);

                case TokenType.Identifier:
                    ReplacementExpression rep = new ReplacementExpression(Take().Text);
                    _replacements.Add(rep.Name);
                    return rep;

                case TokenType.OpenParen:
                    return ParseGroupingExpression();

                default:
                    throw new LangException($"Unexpected token '{_currentToken.Text}'.");
            }
        }

        private Expression ParseGroupingExpression()
        {
            Take();

            Expression expr = ParseBinaryExpression();
            Match(TokenType.CloseParen);

            return new GroupingExpression(expr);
        }

        public Token Match(TokenType tokenType)
        {
            return _currentToken.Type == tokenType
                ? Take()
                : throw new LangException($"Expected '{GetString(tokenType)}' but found '{_currentToken.Text}'.");
        }

        public Token Take()
        {
            Token previous = _currentToken;
            _currentToken = _lexer.Lex();
            return previous;
        }

        public bool IsEndOfStatement()
        {
            return _currentToken.Type == TokenType.EndOfCode;
        }

        private string GetString(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Identifier => "variable",
                TokenType.Number => "number",
                TokenType.Operator => "operator",
                TokenType.EndOfCode => "\0",
                TokenType.OpenParen => "(",
                TokenType.CloseParen => ")",
                TokenType.Exclamation => "!",
                _ => tokenType.ToString(),
            };
        }
    }
}