using System;

namespace StringMath
{
    /// <summary>A calculator.</summary>
    public interface ICalculator : IVariablesCollection
    {
        /// <summary>Add a new binary operator or overwrite an existing operator implementation.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence"><see cref="Precedence.UserDefined"/> precedence by default.</param>
        void AddOperator(string operatorName, Func<double, double, double> operation, Precedence? precedence = default);

        /// <summary>Add a new unary operator or overwrite an existing operator implementation. <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        void AddOperator(string operatorName, Func<double, double> operation);

        /// <summary>Creates an operation that can be evaluated later.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The new operation.</returns>
        OperationInfo CreateOperation(string expression);

        /// <summary>Evaluates a cached mathematical expression.</summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        double Evaluate(OperationInfo operation);

        /// <summary>Evaluates a math expression which can contain variables and returns a double.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double.</returns>
        double Evaluate(string expression);
    }

    /// <inheritdoc />
    public class Calculator : VariablesCollection, ICalculator
    {
        private readonly IMathContext _mathContext;
        private readonly IExpressionReducer _reducer;

        /// <summary>Create an instance of a <see cref="Calculator"/> which has it's own operators and variables.</summary>
        /// <param name="variables">A collection of variables.</param>
        public Calculator(IVariablesCollection? variables = default)
        {
            _mathContext = new MathContext();
            _reducer = new ExpressionReducer(_mathContext, this);

            this["PI"] = Math.PI;
            this["E"] = Math.E;

            variables?.CopyTo(this);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<double, double, double> operation, Precedence? precedence = default)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.AddBinaryOperator(operatorName, operation, precedence);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<double, double> operation)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.AddUnaryOperator(operatorName, operation);
        }

        /// <inheritdoc />
        public double Evaluate(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            ITokenizer tokenizer = new Tokenizer(expression);
            IParser parse = new Parser(tokenizer, _mathContext);

            ValueExpression resultExpr = _reducer.Reduce<ValueExpression>(parse.Parse());
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public double Evaluate(OperationInfo operation)
        {
            operation.EnsureNotNull(nameof(operation));

            ValueExpression resultExpr = _reducer.Reduce<ValueExpression>(operation.Root);
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public OperationInfo CreateOperation(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            ITokenizer tokenizer = new Tokenizer(expression);
            IParser parser = new Parser(tokenizer, _mathContext);
            IExpressionOptimizer optimizer = new ExpressionOptimizer(_reducer, _mathContext);
            Expression root = parser.Parse();
            Expression optimized = optimizer.Optimize(root);

            return new OperationInfo(optimized, expression, parser.Variables);
        }
    }
}
