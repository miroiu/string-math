using SM = StringMath.Expressions;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace StringMath
{
    internal class CompileExpression
    {
        private static readonly ParameterExpression _contextParam = Expression.Parameter(typeof(IMathContext), "__internal_ctx");
        private List<ParameterExpression> _parameters = new List<ParameterExpression>();
        private readonly IVariablesCollection _variables;

        public CompileExpression(IVariablesCollection variables)
        {
            _variables = variables;
        }

        public T Compile<T>(SM.IExpression expression, params string[] parameters)
            => VisitWithParameters<T>(expression, parameters).Compile();

        private Expression<T> VisitWithParameters<T>(SM.IExpression expression, params string[] parameters)
        {
            _parameters = new List<ParameterExpression>(1 + parameters.Length)
            {
                _contextParam
            };

            foreach (var parameter in parameters)
            {
                _parameters.Add(Expression.Parameter(typeof(double), parameter));
            }

            var result = Visit(expression);
            return Expression.Lambda<T>(result, _parameters);
        }

        public Expression Visit(SM.IExpression expression)
        {
            Expression result = expression switch
            {
                SM.BinaryExpression binaryExpr => VisitBinaryExpr(binaryExpr),
                SM.ConstantExpression constantExpr => VisitConstantExpr(constantExpr),
                SM.UnaryExpression unaryExpr => VisitUnaryExpr(unaryExpr),
                SM.VariableExpression variableExpr => VisitVariableExpr(variableExpr),
                _ => throw new NotImplementedException($"'{expression?.GetType().Name}' Convertor is not implemented.")
            };

            return result;
        }

        private Expression VisitVariableExpr(SM.VariableExpression variableExpr)
        {
            var parameter = _parameters.FirstOrDefault(x => x.Name == variableExpr.Name);
            if (parameter != null)
                return parameter;

            if(_variables.TryGetValue(variableExpr.Name, out var variable))
                return Expression.Constant(variable);

            throw MathException.MissingVariable(variableExpr.Name);
        }

        private Expression VisitConstantExpr(SM.ConstantExpression constantExpr) => Expression.Constant(constantExpr.Value);

        private Expression VisitBinaryExpr(SM.BinaryExpression binaryExpr) =>
            Expression.Call(_contextParam,
                nameof(IMathContext.EvaluateBinary),
                null,
                Expression.Constant(binaryExpr.OperatorName), Visit(binaryExpr.Left), Visit(binaryExpr.Right));

        private Expression VisitUnaryExpr(SM.UnaryExpression unaryExpr) =>
            Expression.Call(_contextParam,
                nameof(IMathContext.EvaluateUnary),
                null,
                Expression.Constant(unaryExpr.OperatorName), Visit(unaryExpr.Operand));
    }
}
