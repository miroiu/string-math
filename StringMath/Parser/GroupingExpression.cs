using System;

namespace StringMath
{
    internal sealed class GroupingExpression : Expression
    {
        public GroupingExpression(Expression inner)
            => Inner = inner;

        public Expression Inner { get; }
        public override Type Type => typeof(GroupingExpression);

        public override string ToString()
        {
            return $"({Inner})";
        }
    }
}
