using System;
using System.Collections.Generic;

namespace StringMath
{
    public static class SMath
    {
        private static readonly Calculator _calculator = new Calculator();

        /// <summary>
        /// Add a new binary operator or overwrite an existing operator's logic. 
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence">Logarithmic precedence by default.</param>
        public static void AddOperator(string operatorName, Func<double, double, double> operation, Precedence precedence = default)
        {
            _calculator.AddOperator(operatorName, operation, precedence);
        }

        /// <summary>
        /// Add a new unary operator or overwrite an existing operator's logic. 
        /// <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        public static void AddOperator(string operatorName, Func<double, double> operation)
        {
            _calculator.AddOperator(operatorName, operation);
        }

        /// <summary>
        /// Evaluates a mathematical expression and returns a double value.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public static double Evaluate(string expression)
        {
            return _calculator.Evaluate(expression);
        }

        /// <summary>
        /// Evaluates a mathematical expression that contains variables and returns a double value.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <param name="variables">The variables to be replaced with their values.</param>
        /// <returns>The result as a double value.</returns>
        public static double Evaluate(string expression, VariablesCollection variables)
        {
            foreach (KeyValuePair<string, double> repl in variables)
            {
                _calculator.Replace(repl.Key, repl.Value);
            }

            return Evaluate(expression);
        }

        /// <summary>
        /// Evaluates a cached mathematical expression.
        /// </summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public static double Evaluate(OperationInfo operation)
        {
            return _calculator.Evaluate(operation);
        }

        /// <summary>
        /// Creates an operation that can be evaluated later.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        public static OperationInfo CreateOperation(string expression)
        {
            return _calculator.CreateOperation(expression);
        }

        /// <summary>
        /// Replaces the value of a variable.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The new value.</param>
        public static void Replace(string name, double value)
        {
            _calculator.Replace(name, value);
        }
    }
}
