using System;
using System.Collections.Generic;

namespace StringMath
{
    internal interface IParser
    {
        IReadOnlyCollection<string> Variables { get; }
        Expression Parse();
    }

    internal sealed class Parser : IParser
    {
        private readonly ITokenizer _tokenzier;
        private readonly IMathContext _mathContext;
        private Token _currentToken;
        private readonly HashSet<string> _variables = new HashSet<string>(StringComparer.Ordinal);

        public IReadOnlyCollection<string> Variables => _variables;

        public Parser(ITokenizer tokenizer, IMathContext mathContext)
        {
            _tokenzier = tokenizer;
            _mathContext = mathContext;
        }

        public Expression Parse()
        {
            _currentToken = _tokenzier.ReadToken();
            Expression result = ParseBinaryExpression();
            Match(TokenType.EndOfCode);
            return result;
        }

        private Expression ParseBinaryExpression(Expression? left = default, Precedence? parentPrecedence = default)
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

        public Expression ParsePrimaryExpression()
        {
            switch (_currentToken.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take().Text);

                case TokenType.Identifier:
                    VariableExpression rep = new VariableExpression(Take().Text);
                    _variables.Add(rep.Name);
                    return rep;

                case TokenType.OpenParen:
                    return ParseGroupingExpression();

                default:
                    throw LangException.UnexpectedToken(_currentToken);
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
                : throw LangException.UnexpectedToken(_currentToken, tokenType);
        }

        public Token Take()
        {
            Token previous = _currentToken;
            _currentToken = _tokenzier.ReadToken();
            return previous;
        }

        public bool IsEndOfStatement()
        {
            return _currentToken.Type == TokenType.EndOfCode;
        }
    }
}