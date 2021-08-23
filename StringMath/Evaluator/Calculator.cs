using System;

namespace StringMath
{
    public class Calculator
    {
        private readonly MathContext _mathContext = new MathContext();
        private readonly Replacements _replacements;

        private static Reducer Reducer { get; } = new Reducer();

        /// <summary>
        /// Create an instance of a Calculator which has it's own operators and variable definitions.
        /// </summary>
        /// <param name="replacements"></param>
        public Calculator(Replacements replacements = default)
            => _replacements = replacements ?? new Replacements
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
            SourceText text = new SourceText(expression);
            Lexer lex = new Lexer(text, _mathContext);
            Parser parse = new Parser(lex, _mathContext);

            return Reducer.Reduce<ResultExpression>(parse.Parse(), _mathContext, _replacements).Value;
        }

        /// <summary>
        /// Evaluates a cached mathematical expression.
        /// </summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public double Evaluate(OperationInfo operation)
        {
            return Reducer.Reduce<ResultExpression>(operation.Root, _mathContext, _replacements).Value;
        }

        /// <summary>
        /// Creates an operation that can be evaluated later.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public OperationInfo CreateOperation(string expression)
        {
            SourceText text = new SourceText(expression);
            Lexer lexer = new Lexer(text, _mathContext);
            Parser parser = new Parser(lexer, _mathContext);
            Expression root = parser.Parse();

            return new OperationInfo(root, expression, parser.Replacements);
        }

        /// <summary>
        /// Replaces the value of a variable.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The new value.</param>
        public void Replace(string name, double value)
        {
            _replacements[name] = value;
        }

        /// <summary>
        /// Gets or sets the value of a variable.
        /// </summary>
        /// <param name="replacement">Name of the variable.</param>
        /// <returns>Value of the variable.</returns>
        public double this[string replacement]
        {
            get => _replacements[replacement];
            set => _replacements[replacement] = value;
        }
    }
}
