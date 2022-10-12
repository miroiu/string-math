namespace StringMath.Expressions;

public static class SMath
{
    public static Variable Var(string name) => new(name);
    public static Binary Binary(Expr left, string op, Expr right) => Factory.BinaryExpr(left, op, right);
    public static Unary Unary(Expr left, string op) => Factory.UnaryExpr(op, left);

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