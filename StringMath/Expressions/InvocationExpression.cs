using System.Collections.Generic;
using System.Linq;

namespace StringMath.Expressions
{
    internal class InvocationExpression : IExpression
    {
        public InvocationExpression(string name, IReadOnlyCollection<IExpression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public ExpressionType Type => ExpressionType.Invocation;

        public string Name { get; }
        public IReadOnlyCollection<IExpression> Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
            => ToString(MathContext.Default);

        public string ToString(IMathContext context)
            => $"{Name}({string.Join(", ", Arguments.Select(x => x.ToString()))})";
    }
}
