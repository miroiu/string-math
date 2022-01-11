using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>An optimized and cached math expression.</summary>
    public sealed class OperationInfo<TNum> where TNum : INumber<TNum>
    {
        /// <summary>Initializez a new instance of an operation info.</summary>
        /// <param name="root">The optimized expression tree.</param>
        /// <param name="expression">The math expression string.</param>
        /// <param name="variables">A collection of variables extracted from the expression.</param>
        private OperationInfo(IExpression root, string expression, IReadOnlyCollection<string> variables)
        {
            Root = root;
            Expression = expression;
            Variables = variables;
        }

        /// <summary>The cached expression tree.</summary>
        internal IExpression Root { get; }

        /// <summary>The math expression that was used to create this operation.</summary>
        public string Expression { get; }

        /// <summary>A collection of variable names extracted from the <see cref="Expression"/>.</summary>
        public IReadOnlyCollection<string> Variables { get; }

        /// <summary>Creates a new instance of an operation info.</summary>
        /// <param name="expression">The math expression.</param>
        /// <param name="context">The math context.</param>
        /// <returns>An operation info.</returns>
        internal static OperationInfo<TNum> Create(string expression, IMathContext<TNum> context)
        {
            ITokenizer tokenizer = new Tokenizer(expression);
            IParser parser = new Parser(tokenizer, context);
            IExpressionVisitor<IExpression> optimizer = new ExpressionOptimizer<TNum>(context);
            IExpression root = parser.Parse();
            IExpression optimized = optimizer.Visit(root);

            return new OperationInfo<TNum>(optimized, expression, parser.Variables);
        }
    }
}
