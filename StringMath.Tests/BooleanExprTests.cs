using Xunit;
using System;

namespace StringMath.Tests
{
    public class BooleanExprTests
    {
        private readonly MathContext _context;

        public BooleanExprTests()
        {
            MathExpr.AddVariable("true", 1);
            MathExpr.AddVariable("false", 0);

            _context = new MathContext();
            _context.RegisterLogical("and", (a, b) => a && b, Precedence.Multiplication);
            _context.RegisterLogical("or", (a, b) => a || b, Precedence.Addition);
            _context.RegisterLogical(">", (a, b) => a > b, Precedence.Power);
            _context.RegisterLogical("<", (a, b) => a < b, Precedence.Power);
            _context.RegisterLogical("!", (a) => !a);
        }

        [Fact]
        public void Evaluate_Variable_Substitution()
        {
            MathExpr expr = new MathExpr("{a} and 1", _context);
            Assert.False(expr.Substitute("a", false).EvalBoolean());
            Assert.True(expr.Substitute("a", true).EvalBoolean());
        }

        [Fact]
        public void Evaluate_Boolean_Numbers()
        {
            bool expr = "1 and 1".EvalBoolean(_context);
            Assert.True(expr);

            bool result = "1 and 0 or !0 and 3 > 2".EvalBoolean(_context);
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_Globals_Variables()
        {
            Assert.True("{true} or {false} and {true}".EvalBoolean(_context));
            Assert.True("{true} or {false}".EvalBoolean(_context));
            Assert.False("{false} or {false}".EvalBoolean(_context));
            Assert.False("{true} and {false}".EvalBoolean(_context));
            Assert.True("{true} and {true}".EvalBoolean(_context));
        }

        [Fact]
        public void Evaluate_Binary_Operation()
        {
            _context.RegisterBinary("+", (a, b) => a + b);
            Assert.True("(3 + 5) > 7".EvalBoolean(_context));
        }
    }

    static class BooleanMathExtensions
    {
        public static bool EvalBoolean(this string value) => value.ToMathExpr().EvalBoolean();

        public static bool EvalBoolean(this string value, IMathContext context) => value.ToMathExpr(context).EvalBoolean();

        public static bool EvalBoolean(this MathExpr value) => value.Result != 0;

        public static MathExpr Substitute(this MathExpr expr, string name, bool value)
            => expr.Substitute(name, value is true ? 1 : 0);

        public static void RegisterLogical(this IMathContext context, string operatorName, Func<bool, bool, bool> operation, Precedence? precedence)
            => context.RegisterBinary(operatorName, (a, b) => operation(a != 0, b != 0) ? 1 : 0, precedence);

        public static void RegisterLogical(this IMathContext context, string operatorName, Func<double, double, bool> operation, Precedence? precedence)
            => context.RegisterBinary(operatorName, (a, b) => operation(a, b) ? 1 : 0, precedence);

        public static void RegisterLogical(this IMathContext context, string operatorName, Func<bool, bool> operation)
            => context.RegisterUnary(operatorName, (a) => operation(a != 0) ? 1 : 0);
    }
}
