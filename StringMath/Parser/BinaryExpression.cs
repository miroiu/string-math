namespace StringMath
{
    internal class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, string operatorName, Expression right)
        {
            Left = left;
            OperatorType = operatorName;
            Right = right;
        }

        public Expression Left { get; }
        public string OperatorType { get; }
        public Expression Right { get; }
    }
}
