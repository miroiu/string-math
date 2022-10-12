using StringMath.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMath
{
    /// <summary>A mathematical expression.</summary>
    public class MathExpr
    {
        private readonly IExpressionVisitor _evaluator;
        private readonly IVariablesCollection _variables = new VariablesCollection();
        private IReadOnlyCollection<string>? _localVariables;
        private IReadOnlyCollection<string>? _allVariables;
        private double? _cachedResult;

        internal IExpression Expression { get; }

        /// <summary>The <see cref="IMathContext"/> in which this expression is evaluated.</summary>
        public IMathContext Context { get; }

        /// <summary>A collection of variable names excluding globals from <see cref="Variables"/>.</summary>
        public IReadOnlyCollection<string> LocalVariables => _localVariables ??= Variables.Where(x => !VariablesCollection.Default.Contains(x)).ToList();

        /// <summary>A collection of variable names including globals extracted from the <see cref="Text"/>.</summary>
        public IReadOnlyCollection<string> Variables
        {
            get
            {
                if (_allVariables == null)
                {
                    var extractor = new ExtractVariables();
                    extractor.Visit(Expression);
                    _allVariables = extractor.Variables;
                }

                return _allVariables;
            }
        }

        /// <summary>Constructs a <see cref="MathExpr"/> from a string.</summary>
        /// <param name="text">The math expression.</param>
        public MathExpr(string text) : this(text, new MathContext(MathContext.Default))
        {
        }

        /// <summary>Constructs a <see cref="MathExpr"/> from a string.</summary>
        /// <param name="text">The math expression.</param>
        /// <param name="context">The <see cref="IMathContext"/> in which this expression is evaluated.</param>
        public MathExpr(string text, IMathContext context) : this(text.Parse(context), context)
        {
        }

        /// <summary>Constructs a <see cref="MathExpr"/> from an expression tree.</summary>
        /// <param name="expression">The expression tree.</param>
        /// <param name="context">The <see cref="IMathContext"/> in which this expression is evaluated.</param>
        internal MathExpr(IExpression expression, IMathContext context)
        {
            expression.EnsureNotNull(nameof(expression));
            context.EnsureNotNull(nameof(context));

            Context = context;

            Expression = expression;
            _evaluator = new EvaluateExpression(Context, _variables);
        }

        /// <summary>The result of the expression.</summary>
        /// <remarks>The variables used in the expression must be set before getting the result.</remarks>
        public double Result
        {
            get
            {
                if (!_cachedResult.HasValue)
                {
                    var result = (ConstantExpression)_evaluator.Visit(Expression);
                    _cachedResult = result.Value;
                }

                return _cachedResult.Value;
            }
        }

        /// <summary>Creates a string representation of the current expression.</summary>
        public string Text => Expression.ToString(Context);

        /// <summary>Substitutes the variable with the given value.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value.</param>
        public MathExpr Substitute(string name, double value)
        {
            if (VariablesCollection.Default.TryGetValue(name, out _))
            {
                throw MathException.ReadonlyVariable(name);
            }

            if (LocalVariables.Contains(name))
            {
                _cachedResult = null;
                _variables[name] = value;
            }
            else
            {
                throw MathException.MissingVariable(name);
            }

            return this;
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double> Compile()
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double>>(Expression).Compile();
            return () => exp(Context);
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double, double> Compile(string var)
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double, double>>(Expression, var).Compile();
            return (double x) => exp(Context, x);
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double, double, double> Compile(string var1, string var2)
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double, double, double>>(Expression, var1, var2).Compile();
            return (x, y) => exp(Context, x, y);
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double, double, double, double> Compile(string var1, string var2, string var3)
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double, double, double, double>>(Expression, var1, var2, var3).Compile();
            return (x, y, z) => exp(Context, x, y, z);
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double, double, double, double, double> Compile(string var1, string var2, string var3, string var4)
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double, double, double, double, double>>(Expression, var1, var2, var3, var4).Compile();
            return (x, y, z, w) => exp(Context, x, y, z, w);
        }

        /// <summary>Compiles a <see cref="MathExpr"/> into a delegate.</summary>
        /// <returns>A type safe delegate.</returns>
        public Func<double, double, double, double, double, double> Compile(string var1, string var2, string var3, string var4, string var5)
        {
            var exp = new CompileExpression().Compile<Func<IMathContext, double, double, double, double, double, double>>(Expression, var1, var2, var3, var4, var5).Compile();
            return (x, y, z, w, q) => exp(Context, x, y, z, w, q);
        }

        /// <summary>Converts a string to a <see cref="MathExpr"/>.</summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator MathExpr(string value) => new MathExpr(value);

        /// <summary>Evaluates a <see cref="MathExpr"/>.</summary>
        /// <param name="expression"></param>
        public static implicit operator double(MathExpr expression) => expression.Result;

        /// <inheritdoc cref="Substitute(string, double)" />
        public double this[string name]
        {
            set => Substitute(name, value);
        }

        /// <summary>Add a new binary operator or overwrite an existing operator implementation.</summary>
        /// <param name="name">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence"><see cref="Precedence.UserDefined"/> precedence by default.</param>
        /// <returns>The current math expression.</returns>
        /// <remarks>Operators are inherited from <see cref="AddOperator(string, Func{double, double, double}, Precedence?)"/>.</remarks>
        public MathExpr SetOperator(string name, Func<double, double, double> operation, Precedence? precedence = default)
        {
            _cachedResult = null;
            Context.RegisterBinary(name, operation, precedence);
            return this;
        }

        /// <summary>Add a new unary operator or overwrite an existing operator implementation. <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />.</summary>
        /// <param name="name">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <returns>The current math expression.</returns>
        /// <remarks>Operators are inherited from <see cref="AddOperator(string, Func{double, double})"/>.</remarks>
        public MathExpr SetOperator(string name, Func<double, double> operation)
        {
            _cachedResult = null;
            Context.RegisterUnary(name, operation);
            return this;
        }

        /// <summary>Add a new binary operator or overwrite an existing operator implementation.</summary>
        /// <param name="name">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence"><see cref="Precedence.UserDefined"/> precedence by default.</param>
        /// <remarks>Operators will be available in all <see cref="MathExpr"/> expressions.</remarks>
        public static void AddOperator(string name, Func<double, double, double> operation, Precedence? precedence = default)
            => MathContext.Default.RegisterBinary(name, operation, precedence);

        /// <summary>Add a new unary operator or overwrite an existing operator implementation. <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />.</summary>
        /// <param name="name">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <returns>The current math expression.</returns>
        /// <remarks>Operators will be available in all <see cref="MathExpr"/> expressions. Operators are inherited from <see cref="AddOperator(string, Func{double, double})"/>.</remarks>
        public static void AddOperator(string name, Func<double, double> operation)
            => MathContext.Default.RegisterUnary(name, operation);

        /// <inheritdoc cref="Substitute(string, double)"/>
        /// <remarks>Variables will be available in all <see cref="MathExpr"/> expressions.</remarks>
        public static void AddVariable(string name, double value)
            => VariablesCollection.Default[name] = value;

        /// <inheritdoc cref="Text"/>
        public override string ToString() => Text;
    }
}
