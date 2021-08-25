using System;

namespace StringMath
{
    internal sealed class VariableExpression : Expression
    {
        public VariableExpression(string name)
            => Name = name[1..^1];

        public string Name { get; }
        public override Type Type => typeof(VariableExpression);

        public override string ToString()
        {
            return $"{{{Name}}}";
        }
    }
}
