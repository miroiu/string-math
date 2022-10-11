namespace StringMath.Expressions;

public static partial class Extensions
{
    public static Expr ToExpr(this MathExpr expr)
        => ExprConversion.Convert(expr.Expression);

    public static MathExpr ToMathExpr(this Expr expr, IMathContext? context = default)
    {
        var expression = ExprConversion.Convert(expr);
        return new MathExpr(expression, context ?? MathContext.Default);
    }

    public static MathExpr Expand(this string expr)
        => ((MathExpr)expr).Expand();

    /// <summary>Attempts to simplify the given expression.</summary>
    /// <param name="expr">The expression.</param>
    /// <returns>A new simplified expression.</returns>
    public static MathExpr Simplify(this string expr)
        => ((MathExpr)expr).Simplify();

    public static MathExpr Expand(this MathExpr expr)
    {
        var expanded = expr.ToExpr().Expand();
        return expanded.ToMathExpr(expr.Context);
    }

    /// <summary>Attempts to simplify the given expression.</summary>
    /// <param name="expr">The expression.</param>
    /// <returns>A new simplified expression.</returns>
    public static MathExpr Simplify(this MathExpr expr)
    {
        Expr simplified = expr.ToExpr();
        Expr temp;

        do
        {
            temp = simplified;
            simplified = simplified.Simplify(expr.Context);
        }
        while (simplified != temp);

        return simplified.ToMathExpr(expr.Context);
    }
}

public static partial class Extensions
{
    public static Expr Expand(this Expr expr)
    {
        static Expr E(Expr e) => e switch
        {
            // 5 * (a + b) => 5 * a + 5 * b
            Mul(Number num, Sum sum) => num * E(sum.Left) + num * E(sum.Right),

            // (a + b) * 5 => 5 * a + 5 * b
            Mul(Sum sum, Number num) => num * E(sum.Left) + num * E(sum.Right),

            // (a - b) * 5 => 5 * a - 5 * b
            Mul(Number num, Diff diff) => num * E(diff.Left) - num * E(diff.Right),

            // 5 * (a - b) => 5 * a - 5 * b
            Mul(Diff diff, Number num) => num * E(diff.Left) - num * E(diff.Right),

            // c * (a + b) => c * a + c * b
            Mul(Variable num, Sum sum) => num * E(sum.Left) + num * E(sum.Right),

            // (a + b) * c => c * a + c * b
            Mul(Sum sum, Variable num) => num * E(sum.Left) + num * E(sum.Right),

            // c * (a - b) => c * a - c * b
            Mul(Variable num, Diff diff) => num * E(diff.Left) - num * E(diff.Right),

            // (a - b) * c => c * a - c * b
            Mul(Diff diff, Variable num) => num * E(diff.Left) - num * E(diff.Right),

            // x^(m + n)
            Pow(Expr x, Sum(Expr m, Expr n)) => (E(x) ^ E(m)) * (E(x) ^ E(n)),

            // x^(m - n)
            Pow(Expr x, Diff(Expr m, Expr n)) => (E(x) ^ E(m)) / (E(x) ^ E(n)),

            // (x^m)^n
            Pow(Pow(Expr x, Expr m), Expr n) => E(x) ^ (E(m) * E(n)),

            // (x/y)^-m
            Pow(Div(Expr x, Expr y), Neg(Expr m)) => (E(y) / E(x)) ^ E(m),

            // (x*y)^m
            Pow(Mul(Expr x, Expr y), Expr m) => (E(x) ^ E(m)) * (E(y) ^ E(m)),

            // (x*y)^m
            Pow(Div(Expr x, Expr y), Expr m) => (E(x) ^ E(m)) / (E(y) ^ E(m)),

            // x^-m
            Pow(Expr x, Neg(Expr m)) => 1 / (E(x) ^ E(m)),

            // x^2 => x * x
            Pow(Expr x, Number(2)) => E(x) * E(x),

            _ => e
        };

        return E(expr);
    }

    /// <summary>Attempts to simplify the given expression.</summary>
    /// <param name="expr">The expression.</param>
    /// <returns>A new simplified expression.</returns>
    public static Expr Simplify(this Expr expr, IMathContext context)
    {
        Expr S(Expr e) => e switch
        {
            // -1
            Unary(Number a, string op) => context.EvaluateUnary(op, a.Value),

            // -a
            Unary(Variable, string) c => c,

            // 3 * 2
            Binary(Number a, string op, Number b) => context.EvaluateBinary(op, a.Value, b.Value),

            // x / 2
            Binary(Variable, string, Number) c => c,

            // 2 % x
            Binary(Number, string, Variable) c => c,

            // x ^ y
            Binary(Variable, string, Variable) c => c,

            // a - 0
            Diff(Expr expr, Number(0)) => S(expr),

            // 0 - a
            Diff(Number(0), Expr expr) => S(-expr),

            // 0 + a
            Sum(Number(0), Expr expr) => S(expr),

            // a + 0
            Sum(Expr expr, Number(0)) => S(expr),

            // 0 * a
            Mul(Number(0), Expr) => 0,

            // a * 0
            Mul(Expr, Number(0)) => 0,

            // 1 * a
            Mul(Number(1), Expr expr) => S(expr),

            // a * 1
            Mul(Expr expr, Number(1)) => S(expr),

            // x^1
            Pow(Expr x, Number(1)) => S(x),

            // x^0
            Pow(Expr, Number(0)) => 1,

            // generic
            Binary(Expr a, string op, Expr b) => SMath.Binary(S(a), op, S(b)),
            Unary(Expr a, string op) => SMath.Unary(S(a), op),

            _ => e
        };

        return S(expr);
    }
}

public static class SMath
{
    public static Variable Var(string name) => new(name);
    public static Binary Binary(Expr left, string op, Expr right) => new(left, op, right);
    public static Unary Unary(Expr left, string op) => new(left, op);

    public static Neg Neg(Expr Value) => new(Value);
    public static Factorial Factorial(Expr Value) => new(Value);
    public static Sin Sin(Expr Value) => new(Value);
    public static Cos Cos(Expr Value) => new(Value);
    public static Abs Abs(Expr Value) => new(Value);
    public static Sqrt Sqrt(Expr Value) => new(Value);
    public static Tan Tan(Expr Value) => new(Value);
    public static Atan Atan(Expr Value) => new(Value);
    public static Exp Exp(Expr Value) => new(Value);

    public static Sum Sum(Expr Left, Expr Right) => new(Left, Right);
    public static Diff Diff(Expr Left, Expr Right) => new(Left, Right);
    public static Mul Mul(Expr Left, Expr Right) => new(Left, Right);
    public static Div Div(Expr Left, Expr Right) => new(Left, Right);
    public static Pow Pow(Expr Left, Expr Right) => new(Left, Right);
    public static Mod Mod(Expr Left, Expr Right) => new(Left, Right);
    public static Log Log(Expr Left, Expr Right) => new(Left, Right);
}