using StringMath.Expressions;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>A simple parser.</summary>
    internal sealed class Parser
    {
        private readonly Tokenizer _tokenzier;
        private readonly IMathContext _mathContext;
        private Token _currentToken;

        /// <summary>Initializes a new instance of a parser.</summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="mathContext">The math context.</param>
        public Parser(Tokenizer tokenizer, IMathContext mathContext)
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

        private IExpression ParsePrimaryExpression() => _currentToken.Type switch
        {
            TokenType.Number => new ConstantExpression(Take().Text),
            TokenType.Operator when _mathContext.IsFunction(_currentToken.Text) => ParseInvocationExpression(),
            TokenType.Identifier => new VariableExpression(Take().Text),
            TokenType.OpenParen => ParseGroupingExpression(),
            _ => throw MathException.UnexpectedToken(_currentToken),
        };

        private IExpression ParseInvocationExpression()
        {
            var fnName = Take().Text;

            Match(TokenType.OpenParen);

            var arguments = new List<IExpression>(2);

            do
            {
                if (_currentToken.Text == ",")
                    Take();

                var argExpr = ParseBinaryExpression();
                arguments.Add(argExpr);
            }
            while (_currentToken.Text == ",");

            Match(TokenType.CloseParen);

            return new InvocationExpression(fnName, arguments);
        }

        private IExpression ParseGroupingExpression()
        {
            Take();

            IExpression expr = ParseBinaryExpression();
            Match(TokenType.CloseParen);

            return expr;
        }

        private Token Match(TokenType tokenType) => _currentToken.Type == tokenType
            ? Take()
            : throw MathException.UnexpectedToken(_currentToken, tokenType);

        private Token Take()
        {
            Token previous = _currentToken;
            _currentToken = _tokenzier.ReadToken();
            return previous;
        }

        private bool IsEndOfStatement() 
            => _currentToken.Type == TokenType.EndOfCode;
    }
}