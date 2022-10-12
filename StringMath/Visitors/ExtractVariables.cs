using StringMath.Expressions;
using System;
using System.Collections.Generic;

namespace StringMath
{
    internal class ExtractVariables : BaseExpressionVisitor
    {
        private readonly HashSet<string> _variables = new HashSet<string>(StringComparer.Ordinal);

        public IReadOnlyCollection<string> Variables => _variables;

        protected override IExpression VisitBinaryExpr(BinaryExpression binaryExpr)
        {
            Visit(binaryExpr.Left);
            Visit(binaryExpr.Right);

            return binaryExpr;
        }

        protected override IExpression VisitUnaryExpr(UnaryExpression unaryExpr)
        {
            Visit(unaryExpr.Operand);

            return unaryExpr;
        }

        protected override IExpression VisitVariableExpr(VariableExpression variableExpr)
        {
            _variables.Add(variableExpr.Name);

            return variableExpr;
        }
    }
}
