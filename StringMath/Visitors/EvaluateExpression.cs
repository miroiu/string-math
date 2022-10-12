using StringMath.Expressions;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed class EvaluateExpression : BaseExpressionVisitor
    {
        private readonly IMathContext _context;
        private readonly IVariablesCollection _variables;

        /// <summary>Initializez a new instance of an expression evaluator.</summary>
        /// <param name="context">The math context.</param>
        /// <param name="variables">The variables collection.</param>
        public EvaluateExpression(IMathContext context, IVariablesCollection variables)
        {
            context.EnsureNotNull(nameof(context));
            variables.EnsureNotNull(nameof(variables));

            _variables = variables;
            _context = context;
        }

        protected override IExpression VisitBinaryExpr(BinaryExpression binaryExpr)
        {
            ConstantExpression leftExpr = (ConstantExpression)Visit(binaryExpr.Left);
            ConstantExpression rightExpr = (ConstantExpression)Visit(binaryExpr.Right);

            double result = _context.EvaluateBinary(binaryExpr.OperatorName, leftExpr.Value, rightExpr.Value);
            return new ConstantExpression(result);
        }

        protected override IExpression VisitUnaryExpr(UnaryExpression unaryExpr)
        {
            ConstantExpression valueExpr = (ConstantExpression)Visit(unaryExpr.Operand);

            double result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
            return new ConstantExpression(result);
        }

        protected override IExpression VisitVariableExpr(VariableExpression variableExpr)
        {
            return _variables.TryGetValue(variableExpr.Name, out double value)
                ? new ConstantExpression(value)
                : throw MathException.UnassignedVariable(variableExpr);
        }
    }
}
