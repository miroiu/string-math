using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>A simple parser.</summary>
    internal sealed class Parser : IParser
    {
        private readonly ITokenizer _tokenzier;
        private readonly IOperatorRegistry _opRegistry;
        private Token _currentToken;
        private readonly HashSet<string> _variables = new(StringComparer.Ordinal);

        /// <inheritdoc />
        public IReadOnlyCollection<string> Variables => _variables;

        /// <summary>Initializes a new instance of a parser.</summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="opRegistry">The operator registry.</param>
        public Parser(ITokenizer tokenizer, IOperatorRegistry opRegistry)
        {
            _tokenzier = tokenizer;
            _opRegistry = opRegistry;
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
                if (_opRegistry.IsUnary(_currentToken.Text))
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
                if (_opRegistry.IsBinary(_currentToken.Text))
                {
                    Precedence precedence = _opRegistry.GetBinaryPrecedence(_currentToken.Text);
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
                    VariableExpression rep = new(Take().Text);
                    _variables.Add(rep.Name);
                    return rep;

                case TokenType.OpenParen:
                    return ParseGroupingExpression();

                default:
                    throw LangException.UnexpectedToken(_currentToken);
            }
        }

        private IExpression ParseGroupingExpression()
        {
            Take();

            IExpression expr = ParseBinaryExpression();
            Match(TokenType.CloseParen);

            return new GroupingExpression(expr);
        }

        private Token Match(TokenType tokenType)
        {
            return _currentToken.Type == tokenType
                ? Take()
                : throw LangException.UnexpectedToken(_currentToken, tokenType);
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