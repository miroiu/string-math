namespace StringMath.Expressions;

public record Expr
{
    public static Mul operator *(Expr left, Expr right) => new(left, right);
    public static Div operator /(Expr left, Expr right) => new(left, right);
    public static Sum operator +(Expr left, Expr right) => new(left, right);
    public static Diff operator -(Expr left, Expr right) => new(left, right);
    public static Pow operator ^(Expr left, Expr right) => new(left, right);
    public static Mod operator %(Expr left, Expr right) => new(left, right);
    public static Neg operator -(Expr operand) => new(operand);

    public static implicit operator Expr(double value) => new Number(value);
}

public record Variable(string Name) : Expr;
public record Number(double Value) : Expr
{
    public static implicit operator Number(double val) => new(val);
    public static implicit operator double(Number num) => num.Value;
}
public record Binary(Expr Left, string Op, Expr Right) : Expr;
public record Unary(Expr Value, string Op) : Expr;

public record Neg(Expr Value) : Unary(Value, "-");
public record Factorial(Expr Value) : Unary(Value, "!");
public record Sin(Expr Value) : Unary(Value, "sin");
public record Cos(Expr Value) : Unary(Value, "cos");
public record Abs(Expr Value) : Unary(Value, "abs");
public record Sqrt(Expr Value) : Unary(Value, "sqrt");
public record Tan(Expr Value) : Unary(Value, "tan");
public record Atan(Expr Value) : Unary(Value, "atan");
public record Exp(Expr Value) : Unary(Value, "exp");

public record Sum(Expr Left, Expr Right) : Binary(Left, "+", Right);
public record Diff(Expr Left, Expr Right) : Binary(Left, "-", Right);
public record Mul(Expr Left, Expr Right) : Binary(Left, "*", Right);
public record Div(Expr Left, Expr Right) : Binary(Left, "/", Right);
public record Pow(Expr Left, Expr Right) : Binary(Left, "^", Right);
public record Mod(Expr Left, Expr Right) : Binary(Left, "%", Right);
public record Log(Expr Left, Expr Right) : Binary(Left, "log", Right);
