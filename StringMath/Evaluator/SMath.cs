using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>Calculator static API.</summary>
    public static class SMath
    {
        private static readonly ICalculator _calculator = new Calculator();

        /// <inheritdoc cref="ICalculator.AddOperator(string, Func{double, double, double}, Precedence?)" />
        public static void AddOperator(string operatorName, Func<double, double, double> operation, Precedence? precedence = default)
        {
            _calculator.AddOperator(operatorName, operation, precedence);
        }

        /// <inheritdoc cref="ICalculator.AddOperator(string, Func{double, double})" />
        public static void AddOperator(string operatorName, Func<double, double> operation)
        {
            _calculator.AddOperator(operatorName, operation);
        }

        /// <inheritdoc cref="ICalculator.Evaluate(string)" />
        public static double Evaluate(string expression)
        {
            return _calculator.Evaluate(expression);
        }

        /// <inheritdoc cref="ICalculator.Evaluate(OperationInfo)" />
        public static double Evaluate(OperationInfo operation)
        {
            return _calculator.Evaluate(operation);
        }

        /// <inheritdoc cref="ICalculator.CreateOperation(string)" />
        public static OperationInfo CreateOperation(string expression)
        {
            return _calculator.CreateOperation(expression);
        }

        /// <inheritdoc cref="IVariablesCollection.SetValue(string, double)" />
        public static void SetValue(string name, double value)
        {
            _calculator.SetValue(name, value);
        }

        /// <summary>Add a collection of variables.</summary>
        /// <param name="variables">The variables to be set with their values.</param>
        public static void SetValues(VariablesCollection variables)
        {
            variables.EnsureNotNull(nameof(variables));

            foreach (KeyValuePair<string, double> repl in variables)
            {
                _calculator.SetValue(repl.Key, repl.Value);
            }
        }
    }
}
