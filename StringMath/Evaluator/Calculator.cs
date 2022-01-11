using System;

namespace StringMath
{
    /// <inheritdoc />
    public sealed class Calculator<TNum> : VariablesCollection<TNum>, ICalculator<TNum> where TNum : INumber<TNum>
    {
        private readonly IMathContext<TNum> _mathContext;
        private readonly IExpressionVisitor<ValueExpression<TNum>> _evaluator;

        /// <summary>Create an instance of a <see cref="Calculator{TNum}"/> which has it's own operators and variables.</summary>
        /// <param name="variables">A collection of variables.</param>
        public Calculator(IVariablesCollection<TNum>? variables = default) : this(new MathContext<TNum>(), variables)
        {
        }

        /// <summary>Create an instance of a <see cref="Calculator{TNum}"/> which has it's own operators and variables.</summary>
        /// <param name="context">The math context.</param>
        /// <param name="variables">A collection of variables.</param>
        public Calculator(IMathContext<TNum> context, IVariablesCollection<TNum>? variables = default)
        {
            context.EnsureNotNull(nameof(context));

            _mathContext = context;
            _evaluator = new ExpressionEvaluator<TNum>(_mathContext, this);

            this["PI"] = TNum.Create(Math.PI);
            this["E"] = TNum.Create(Math.E);

            variables?.CopyTo(this);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<TNum, TNum, TNum> operation, Precedence? precedence = default)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.RegisterBinary(operatorName, operation, precedence);
        }

        /// <inheritdoc />
        public void AddOperator(string operatorName, Func<TNum, TNum> operation)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _mathContext.RegisterUnary(operatorName, operation);
        }

        /// <inheritdoc />
        public TNum Evaluate(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            ITokenizer tokenizer = new Tokenizer(expression);
            IParser parse = new Parser(tokenizer, _mathContext);

            ValueExpression<TNum> resultExpr = _evaluator.Visit(parse.Parse());
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public TNum Evaluate(OperationInfo<TNum> operation)
        {
            operation.EnsureNotNull(nameof(operation));

            ValueExpression<TNum> resultExpr = _evaluator.Visit(operation.Root);
            return resultExpr.Value;
        }

        /// <inheritdoc />
        public OperationInfo<TNum> CreateOperation(string expression)
        {
            expression.EnsureNotNull(nameof(expression));

            return OperationInfo<TNum>.Create(expression, _mathContext);
        }
    }
}
