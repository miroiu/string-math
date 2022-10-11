namespace StringMath.Expressions;

public static class Factory
{
    public delegate Binary BinaryExprFactory(Expr left, string op, Expr right);
    public delegate Unary UnaryExprFactory(string op, Expr operand);

    public static BinaryExprFactory BinaryExpr = (left, op, right) => op switch
    {
        "+" => new Sum(left, right),
        "-" => new Diff(left, right),
        "/" => new Div(left, right),
        "*" => new Mul(left, right),
        "^" => new Pow(left, right),
        "%" => new Mod(left, right),
        "log" => new Log(left, right),
        _ => new Binary(left, op, right),
    };

    public static UnaryExprFactory UnaryExpr = (op, operand) => op switch
    {
        "-" => new Neg(operand),
        "!" => new Factorial(operand),
        "sin" => new Sin(operand),
        "cos" => new Cos(operand),
        "abs" => new Abs(operand),
        "tan" => new Tan(operand),
        "atan" => new Atan(operand),
        "sqrt" => new Sqrt(operand),
        "exp" => new Exp(operand),
        _ => new Unary(operand, op)
    };
}

internal static partial class ExprConversion
{
    public static Expr Convert(IExpression expression)
    {
        Expr result = expression switch
        {
            BinaryExpression binaryExpr => ConvertBinaryExpr(binaryExpr),
            ConstantExpression constantExpr => ConvertConstantExpr(constantExpr),
            UnaryExpression unaryExpr => ConvertUnaryExpr(unaryExpr),
            VariableExpression variableExpr => ConvertVariableExpr(variableExpr),
            _ => throw new NotImplementedException($"'{expression.Type}' Convertor is not implemented.")
        };

        return result;
    }

    public static Expr ConvertVariableExpr(VariableExpression variableExpr) => new Variable(variableExpr.Name);

    public static Expr ConvertConstantExpr(ConstantExpression constantExpr) => new Number(constantExpr.Value);

    public static Expr ConvertBinaryExpr(BinaryExpression binaryExpr)
    {
        Expr left = Convert(binaryExpr.Left);
        Expr right = Convert(binaryExpr.Right);

        return Factory.BinaryExpr(left, binaryExpr.OperatorName, right);
    }

    public static Expr ConvertUnaryExpr(UnaryExpression unaryExpr)
    {
        Expr operand = Convert(unaryExpr.Operand);
        return Factory.UnaryExpr(unaryExpr.OperatorName, operand);
    }
}

internal partial class ExprConversion
{
    public static IExpression Convert(Expr expression)
    {
        IExpression result = expression switch
        {
            Binary binaryExpr => ConvertBinaryExpr(binaryExpr),
            Number constantExpr => ConvertConstantExpr(constantExpr),
            Unary unaryExpr => ConvertUnaryExpr(unaryExpr),
            Variable variableExpr => ConvertVariableExpr(variableExpr),
            _ => throw new NotImplementedException($"'{expression?.GetType().Name}' Convertor is not implemented.")
        };

        return result;
    }

    public static IExpression ConvertVariableExpr(Variable variableExpr) => new VariableExpression(variableExpr.Name);

    public static IExpression ConvertConstantExpr(Number constantExpr) => new ConstantExpression(constantExpr.Value);

    public static IExpression ConvertBinaryExpr(Binary binaryExpr) => new BinaryExpression(Convert(binaryExpr.Left), binaryExpr.Op, Convert(binaryExpr.Right));

    public static IExpression ConvertUnaryExpr(Unary unaryExpr) => new UnaryExpression(unaryExpr.Op, Convert(unaryExpr.Value));
}