namespace StringMath.Expressions;

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

            // -a * -b => a * b
            Mul(Neg(Expr a), Neg(Expr b)) => E(a) * E(b),

            // -(a - b) => -a + b
            Neg(Neg(Expr a)) => E(a),

            // -(a - b) => -a + b
            Neg(Diff(Expr a, Expr b)) => -E(a) + E(b),

            // -(a + b) => -a - b
            Neg(Sum(Expr a, Expr b)) => -E(a) - E(b),

            // -a / -b => a / b
            Div(Neg(Expr a), Neg(Expr b)) => E(a) * E(b),

            // x^(m + n) => x^m * x^n
            Pow(Expr x, Sum(Expr m, Expr n)) => (E(x) ^ E(m)) * (E(x) ^ E(n)),

            // x^(m - n) => x^m / x^n
            Pow(Expr x, Diff(Expr m, Expr n)) => (E(x) ^ E(m)) / (E(x) ^ E(n)),

            // (x^m)^n => x^(m*n)
            Pow(Pow(Expr x, Expr m), Expr n) => E(x) ^ (E(m) * E(n)),

            // (x/y)^-m => (y/x)^m
            Pow(Div(Expr x, Expr y), Neg(Expr m)) => (E(y) / E(x)) ^ E(m),

            // (x*y)^m => x^m & y^m
            Pow(Mul(Expr x, Expr y), Expr m) => (E(x) ^ E(m)) * (E(y) ^ E(m)),

            // (x/y)^m => x^m / y^m
            Pow(Div(Expr x, Expr y), Expr m) => (E(x) ^ E(m)) / (E(y) ^ E(m)),

            // x^-m => 1 / x^m
            Pow(Expr x, Neg(Expr m)) => 1 / (E(x) ^ E(m)),

            // x^2 => x * x
            Pow(Expr x, Number(2)) => E(x) * E(x),

            // generic
            Binary(Expr a, string op, Expr b) => SMath.Binary(E(a), op, E(b)),
            Unary(Expr a, string op) => SMath.Unary(E(a), op),

            _ => e
        };

        return E(expr);
    }
}

