namespace SCLang
{
    internal class UnaryExpression : Expression
    {
        public UnaryExpression(string operatorName, Expression operand)
        {
            OperatorType = operatorName;
            Operand = operand;
        }

        public string OperatorType { get; }
        public Expression Operand { get; }
    }
}
