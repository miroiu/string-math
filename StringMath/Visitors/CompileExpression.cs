using SM = StringMath.Expressions;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace StringMath
{
    internal class CompileExpression
    {
        private static readonly ParameterExpression _contextParam = Expression.Parameter(typeof(IMathContext), "ctx");
        private Dictionary<string, ParameterExpression> _variables = new Dictionary<string, ParameterExpression>();

        public Expression<T> Compile<T>(SM.IExpression expression, params string[] variables)
        {
            _variables = variables.ToDictionary(var => var, var => Expression.Parameter(typeof(double), var));
            var args = new List<ParameterExpression>(1 + variables.Length)
            {
                _contextParam
            };
            args.AddRange(_variables.Values);

            var result = Convert(expression);
            return Expression.Lambda<T>(result, args);
        }

        public Expression Convert(SM.IExpression expression)
        {
            Expression result = expression switch
            {
                SM.BinaryExpression binaryExpr => ConvertBinaryExpr(binaryExpr),
                SM.ConstantExpression constantExpr => ConvertConstantExpr(constantExpr),
                SM.UnaryExpression unaryExpr => ConvertUnaryExpr(unaryExpr),
                SM.VariableExpression variableExpr => ConvertVariableExpr(variableExpr),
                _ => throw new NotImplementedException($"'{expression?.GetType().Name}' Convertor is not implemented.")
            };

            return result;
        }

        public Expression ConvertVariableExpr(SM.VariableExpression variableExpr)
        {
            if (!_variables.TryGetValue(variableExpr.Name, out var result))
                throw MathException.MissingVariable(variableExpr.Name);

            return result;
        }

        public Expression ConvertConstantExpr(SM.ConstantExpression constantExpr) => Expression.Constant(constantExpr.Value);

        public Expression ConvertBinaryExpr(SM.BinaryExpression binaryExpr) =>
            Expression.Call(_contextParam,
                nameof(IMathContext.EvaluateBinary),
                null,
                Expression.Constant(binaryExpr.OperatorName), Convert(binaryExpr.Left), Convert(binaryExpr.Right));

        public Expression ConvertUnaryExpr(SM.UnaryExpression unaryExpr) =>
            Expression.Call(_contextParam,
                nameof(IMathContext.EvaluateUnary),
                null,
                Expression.Constant(unaryExpr.OperatorName), Convert(unaryExpr.Operand));
    }
}
