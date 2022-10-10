using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMath
{
    /// <summary>A mathematical expression.</summary>
    public class MathExpr
    {
        private readonly IExpression _expression;
        private readonly IExpressionVisitor<ValueExpression> _evaluator;
        private readonly IVariablesCollection _variables = new VariablesCollection();
        private readonly IMathContext _context;
        private double? _cachedResult;

        /// <summary>A collection of variable names extracted from the <see cref="Text"/>.</summary>
        public IReadOnlyCollection<string> Variables { get; }

        /// <summary>Constructs a <see cref="MathExpr"/> from a string.</summary>
        /// <param name="text">The math expression.</param>
        /// <param name="optimize">Whether to optimize the expression or not.</param>
        public MathExpr(string text, bool optimize = false) : this(text, new MathContext(MathContext.Default), optimize)
        {
        }

        /// <summary>Constructs a <see cref="MathExpr"/> from a string.</summary>
        /// <param name="text">The math expression.</param>
        /// <param name="context">The <see cref="IMathContext"/> in which this expression is evaluated.</param>
        /// <param name="optimize">Whether to optimize the expression or not.</param>
        public MathExpr(string text, IMathContext context, bool optimize = false)
        {
            text.EnsureNotNull(nameof(text));
            context.EnsureNotNull(nameof(context));

            Text = text;
            _context = context;

            ITokenizer tokenizer = new Tokenizer(Text);
            IParser parser = new Parser(tokenizer, context);
            IExpression root = parser.Parse();
            Variables = parser.Variables.Where(x => !VariablesCollection.Default.Contains(x)).ToList();

            if (optimize)
            {
                // TODO: Register pipelines in MathContext
                IExpressionVisitor<IExpression> optimizer = new ExpressionOptimizer(context);
                root = optimizer.Visit(root);
            }

            _expression = root;
            _evaluator = new ExpressionEvaluator(_context, _variables);
        }

        /// <summary>The result of the expression.</summary>
        /// <remarks>The variables used in the expression must be set before getting the result.</remarks>
        public double Result
        {
            get
            {
                if (!_cachedResult.HasValue)
                {
                    var result = _evaluator.Visit(_expression);
                    _cachedResult = result.Value;
                }

                return _cachedResult.Value;
            }
        }

        /// <summary>The string that was used to create this expression.</summary>
        public string Text { get; }

        /// <summary>Substitutes the variable with the given value.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value.</param>
        public MathExpr Substitute(string name, double value)
        {
            if (VariablesCollection.Default.TryGetValue(name, out _))
            {
                throw MathException.ReadonlyVariable(name);
            }

            if (Variables.Contains(name))
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
            _context.RegisterBinary(name, operation, precedence);
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
            _context.RegisterUnary(name, operation);
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
