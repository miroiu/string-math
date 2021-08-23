using System;

namespace StringMath
{
    public interface ICalculator
    {
        double this[string variable] { get; set; }

        void AddOperator(string operatorName, Func<double, double, double> operation, Precedence precedence = null);
        void AddOperator(string operatorName, Func<double, double> operation);
        OperationInfo CreateOperation(string expression);
        double Evaluate(OperationInfo operation);
        double Evaluate(string expression);
        void Replace(string name, double value);
    }

    public class Calculator : ICalculator
    {
        private readonly IMathContext _mathContext = new MathContext();
        private readonly IVariablesCollection _variables;

        private static IExpressionReducer Reducer { get; } = new ExpressionReducer();

        /// <summary>
        /// Create an instance of a Calculator which has it's own operators and variable definitions.
        /// </summary>
        /// <param name="variables"></param>
        public Calculator(IVariablesCollection variables = default)
            => _variables = variables ?? new VariablesCollection
            {
                ["PI"] = Math.PI,
                ["E"] = Math.E
            };

        /// <summary>
        /// Add a new binary operator or overwrite an existing operator's logic. 
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence">Logarithmic precedence by default.</param>
        public void AddOperator(string operatorName, Func<double, double, double> operation, Precedence precedence = default)
        {
            _mathContext.AddBinaryOperator(operatorName, operation, precedence);
        }

        /// <summary>
        /// Add a new unary operator or overwrite an existing operator's logic. 
        /// <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        public void AddOperator(string operatorName, Func<double, double> operation)
        {
            _mathContext.AddUnaryOperator(operatorName, operation);
        }

        /// <summary>
        /// Evaluates a mathematical expression which can contain variables and returns a double value.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public double Evaluate(string expression)
        {
            ISourceText text = new SourceText(expression);
            ILexer lex = new Lexer(text, _mathContext);
            IParser parse = new Parser(lex, _mathContext);

            return Reducer.Reduce<ValueExpression>(parse.Parse(), _mathContext, _variables).Value;
        }

        /// <summary>
        /// Evaluates a cached mathematical expression.
        /// </summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public double Evaluate(OperationInfo operation)
        {
            return Reducer.Reduce<ValueExpression>(operation.Root, _mathContext, _variables).Value;
        }

        /// <summary>
        /// Creates an operation that can be evaluated later.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public OperationInfo CreateOperation(string expression)
        {
            ISourceText text = new SourceText(expression);
            ILexer lexer = new Lexer(text, _mathContext);
            IParser parser = new Parser(lexer, _mathContext);
            IExpressionOptimizer optimizer = new ExpressionOptimizer(Reducer, _mathContext);
            Expression root = parser.Parse();
            Expression optimized = optimizer.Optimize(root);

            return new OperationInfo(optimized, expression, parser.Variables);
        }

        /// <summary>
        /// Replaces the value of a variable.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The new value.</param>
        public void Replace(string name, double value)
        {
            _variables.Set(name, value);
        }

        /// <summary>
        /// Gets or sets the value of a variable.
        /// </summary>
        /// <param name="variable">Name of the variable.</param>
        /// <returns>Value of the variable.</returns>
        public double this[string variable]
        {
            get => _variables.Get(variable);
            set => _variables.Set(variable, value);
        }
    }
}
