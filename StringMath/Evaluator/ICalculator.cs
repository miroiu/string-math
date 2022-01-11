using System;

namespace StringMath
{
    /// <summary>A calculator.</summary>
    public interface ICalculator<TNum> : IVariablesCollection<TNum> where TNum : INumber<TNum>
    {
        /// <summary>Add a new binary operator or overwrite an existing operator implementation.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence"><see cref="Precedence.UserDefined"/> precedence by default.</param>
        void AddOperator(string operatorName, Func<TNum, TNum, TNum> operation, Precedence? precedence = default);

        /// <summary>Add a new unary operator or overwrite an existing operator implementation. <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        void AddOperator(string operatorName, Func<TNum, TNum> operation);

        /// <summary>Creates an operation that can be evaluated later.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The new operation.</returns>
        OperationInfo<TNum> CreateOperation(string expression);

        /// <summary>Evaluates a cached mathematical expression.</summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a TNum value.</returns>
        TNum Evaluate(OperationInfo<TNum> operation);

        /// <summary>Evaluates a math expression which can contain variables and returns a TNum.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a TNum.</returns>
        TNum Evaluate(string expression);
    }
}
