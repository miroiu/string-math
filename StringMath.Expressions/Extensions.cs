namespace StringMath.Expressions;

public static partial class Extensions
{
    public static Expr ToExpr(this MathExpr expr)
        => ExprConversion.Convert(expr.Expression);

    public static Expr ToExpr(this string expr)
        => ExprConversion.Convert(((MathExpr)expr).Expression);

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
        Expr expanded = expr.ToExpr();
        Expr temp;

        do
        {
            temp = expanded;
            expanded = expanded.Expand();
        }
        while (expanded != temp);

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
