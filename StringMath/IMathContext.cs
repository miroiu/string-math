using System;

namespace StringMath
{
    /// <summary>Contract for math context.</summary>
    public interface IMathContext
    {
        /// <summary>The parent math context to inherit operators from.</summary>
        IMathContext? Parent { get; }

        /// <summary>Registers a binary operator implementation.</summary>
        /// <param name="operatorName">The name of the operator.</param>
        /// <param name="operation">The implementation of the operator.</param>
        /// <param name="precedence">The precedence of the operator.</param>
        void RegisterBinary(string operatorName, Func<double, double, double> operation, Precedence? precedence = default);

        /// <summary>Registers an unary operator implementation. Precedence is <see cref="Precedence.Prefix"/>.</summary>
        /// <param name="operatorName">The name of the operator.</param>
        /// <param name="operation">The implementation of the operator.</param>
        void RegisterUnary(string operatorName, Func<double, double> operation);

        /// <summary>Evaluates a binary operation.</summary>
        /// <param name="op">The operator.</param>
        /// <param name="a">Left value.</param>
        /// <param name="b">Right value.</param>
        /// <returns>The result.</returns>
        double EvaluateBinary(string op, double a, double b);

        /// <summary>Evaluates an unary operation.</summary>
        /// <param name="op">The operator.</param>
        /// <param name="a">The value.</param>
        /// <returns>The result.</returns>
        double EvaluateUnary(string op, double a);

        /// <summary>Returns the precedence of a binary operator. Unary operators have <see cref="Precedence.Prefix"/> precedence.</summary>
        /// <param name="operatorName">The operator.</param>
        /// <returns>A <see cref="Precedence"/> value.</returns>
        Precedence GetBinaryPrecedence(string operatorName);

        /// <summary>Tells whether an operator is binary.</summary>
        /// <param name="operatorName">The operator.</param>
        /// <returns>True if the operator is binary, false if it does not exist or it is unary.</returns>
        bool IsBinary(string operatorName);

        /// <summary>Tells whether an operator is unary.</summary>
        /// <param name="operatorName">The operator.</param>
        /// <returns>True if the operator is unary, false if it does not exist or it is binary.</returns>
        bool IsUnary(string operatorName);
    }
}
