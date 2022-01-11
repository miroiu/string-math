using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>Calculator static API.</summary>
    public static class SMath<TNum> where TNum : INumber<TNum>
    {
        private static readonly ICalculator<TNum> _calculator = new Calculator<TNum>();

        /// <inheritdoc cref="ICalculator{TNum}.AddOperator(string, Func{TNum, TNum, TNum}, Precedence?)" />
        public static void AddOperator(string operatorName, Func<TNum, TNum, TNum> operation, Precedence? precedence = default)
        {
            _calculator.AddOperator(operatorName, operation, precedence);
        }

        /// <inheritdoc cref="ICalculator{TNum}.AddOperator(string, Func{TNum, TNum})" />
        public static void AddOperator(string operatorName, Func<TNum, TNum> operation)
        {
            _calculator.AddOperator(operatorName, operation);
        }

        /// <inheritdoc cref="ICalculator{TNum}.Evaluate(string)" />
        public static TNum Evaluate(string expression)
        {
            return _calculator.Evaluate(expression);
        }

        /// <inheritdoc cref="ICalculator{TNum}.Evaluate(OperationInfo{TNum})" />
        public static TNum Evaluate(OperationInfo<TNum> operation)
        {
            return _calculator.Evaluate(operation);
        }

        /// <inheritdoc cref="ICalculator{TNum}.CreateOperation(string)" />
        public static OperationInfo<TNum> CreateOperation(string expression)
        {
            return _calculator.CreateOperation(expression);
        }

        /// <inheritdoc cref="IVariablesCollection{TNum}.SetValue(string, TNum)" />
        public static void SetValue(string name, TNum value)
        {
            _calculator.SetValue(name, value);
        }

        /// <summary>Add a collection of variables.</summary>
        /// <param name="variables">The variables to be set with their values.</param>
        public static void SetValues(VariablesCollection<TNum> variables)
        {
            variables.EnsureNotNull(nameof(variables));

            foreach (KeyValuePair<string, TNum> repl in variables)
            {
                _calculator.SetValue(repl.Key, repl.Value);
            }
        }
    }
}
