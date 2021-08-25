using System;

namespace StringMath
{
    /// <summary>A calculator.</summary>
    public interface ICalculator : IVariablesCollection
    {
        /// <summary>Add a new binary operator or overwrite an existing operator implementation.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence"><see cref="Precedence.UserDefined"/> precedence by default.</param>
        void AddOperator(string operatorName, Func<double, double, double> operation, Precedence? precedence = default);

        /// <summary>Add a new unary operator or overwrite an existing operator implementation. <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />.</summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        void AddOperator(string operatorName, Func<double, double> operation);

        /// <summary>Creates an operation that can be evaluated later.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The new operation.</returns>
        OperationInfo CreateOperation(string expression);

        /// <summary>Evaluates a cached mathematical expression.</summary>
        /// <param name="operation">The math expression to evaluate.</param>
        /// <returns>The result as a double value.</returns>
        double Evaluate(OperationInfo operation);

        /// <summary>Evaluates a math expression which can contain variables and returns a double.</summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a double.</returns>
        double Evaluate(string expression);
    }
}
