namespace StringMath.Expressions;

public static partial class Extensions
{
    /// <summary>Attempts to simplify the given expression.</summary>
    /// <param name="expr">The expression.</param>
    /// <returns>A new simplified expression.</returns>
    public static Expr Simplify(this Expr expr, IMathContext context)
    {
        Expr S(Expr e) => e switch
        {
            // -5 => -5
            Unary(Number a, string op) => context.EvaluateUnary(op, a.Value),

            // 3 * 2 => 6
            Binary(Number a, string op, Number b) => context.EvaluateBinary(op, a.Value, b.Value),

            // a - 0 => a
            Diff(Expr expr, Number(0)) => S(expr),

            // 0 - a => -a
            Diff(Number(0), Expr expr) => -S(expr),

            // 0 + a => a
            Sum(Number(0), Expr expr) => S(expr),

            // a + 0 => a
            Sum(Expr expr, Number(0)) => S(expr),

            // 0 * a => 0
            Mul(Number(0), Expr) => 0,

            // a * 0 => 0
            Mul(Expr, Number(0)) => 0,

            // 1 * a => a
            Mul(Number(1), Expr expr) => S(expr),

            // a * 1 => a
            Mul(Expr expr, Number(1)) => S(expr),

            // x^1 => x
            Pow(Expr x, Number(1)) => S(x),

            // x^0 => 1
            Pow(Expr, Number(0)) => 1,

            // generic
            Binary(Expr a, string op, Expr b) => SMath.Binary(S(a), op, S(b)),
            Unary(Expr a, string op) => SMath.Unary(S(a), op),

            _ => e
        };

        return S(expr);
    }
}
