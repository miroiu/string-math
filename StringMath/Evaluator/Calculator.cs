using System;

namespace StringMath
{
    /// <inheritdoc />
    public sealed class Calculator : VariablesCollection, ICalculator
    {
        private readonly IMathContext _mathContext;
        private readonly IExpressionVisitor<ValueExpression> _evaluator;

        /// <summary>Create an instance of a <see cref="Calculator"/> which has it's own operators and variables.</summary>
        /// <param name="variables">A collection of variables.</param>
        public Calculator(IVariablesCollection? variables = default) : this(new MathContext(), variables)
        {
        }

        /// <summary>Create an instance of a <see cref="Calculator"/> which has it's own operators and variables.</summary>
        /// <param name="context">The math context.</param>
        /// <param name="variables">A collection of variables.</param>
        public Calculator(IMathContext context, IVariablesCollection? variables = default)
        {
            context.EnsureNotNull(nameof(context));

            _mathContext = context;
            _evaluator = new ExpressionEvaluator(_mathContext, this);

            this["PI"] = Math.PI;
            this["E"] = Math.E;

            variables?.CopyTo(this);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<double, double, double> operation, Precedence? precedence = default)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.RegisterBinary(operatorName, operation, precedence);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<double, double> operation)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.RegisterUnary(operatorName, operation);
        }

        /// <inheritdoc />
        public double Evaluate(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            ITokenizer tokenizer = new Tokenizer(expression);
            IParser parse = new Parser(tokenizer, _mathContext);

            ValueExpression resultExpr = _evaluator.Visit(parse.Parse());
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public double Evaluate(OperationInfo operation)
        {
            operation.EnsureNotNull(nameof(operation));

            ValueExpression resultExpr = _evaluator.Visit(operation.Root);
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public OperationInfo CreateOperation(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            return OperationInfo.Create(expression, _mathContext);
        }
    }
}
