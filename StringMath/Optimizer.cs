using System.Collections.Generic;

namespace StringMath
{
    internal class Optimizer
    {
        private readonly Reducer _reducer;
        private readonly MathContext _context;
        private readonly Dictionary<string, double> _variables;

        public Optimizer(Reducer reducer, MathContext mathContext)
        {
            _reducer = reducer;
            _context = mathContext;
        }

        public Expression Optimize(Expression root)
        {
            if (root is ConstantExpression)
            {
                ValueExpression valueExpr = _reducer.Reduce<ValueExpression>(root, _context, _variables);
                return valueExpr;
            }

            if (root is GroupingExpression groupingExpr)
            {
                Expression innerExpr = Optimize(groupingExpr.Inner);
                return innerExpr;
            }

            if (root is BinaryExpression binaryExpr)
            {
                Expression leftExpr = Optimize(binaryExpr.Left);
                Expression rightExpr = Optimize(binaryExpr.Right);

                if (leftExpr is ValueExpression leftValue && rightExpr is ValueExpression rightValue)
                {
                    double result = _context.EvaluateBinary(binaryExpr.OperatorName, leftValue.Value, rightValue.Value);
                    return new ValueExpression(result);
                }

                return new BinaryExpression(leftExpr, binaryExpr.OperatorName, rightExpr);
            }
            
            if (root is UnaryExpression unaryExpr)
            {
                Expression operandExpr = Optimize(unaryExpr.Operand);
                if (operandExpr is ValueExpression valueExpr)
                {
                    double result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
                    return new ValueExpression(result);
                }

                return new UnaryExpression(unaryExpr.OperatorName, operandExpr);
            }

            return root;
        }
    }
}
