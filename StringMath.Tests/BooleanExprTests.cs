using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class BooleanExprTests
    {
        private MathContext _context;

        [SetUp]
        public void Setup()
        {
            MathExpr.AddVariable("true", 1);
            MathExpr.AddVariable("false", 0);

            _context = new MathContext();
            _context.RegisterBinary("and", (a, b) => a != 0 && b != 0 ? 1 : 0, Precedence.Multiplication);
            _context.RegisterBinary("or", (a, b) => a != 0 || b != 0 ? 1 : 0, Precedence.Addition);
            _context.RegisterBinary(">", (a, b) => a > b ? 1 : 0, Precedence.Power);
            _context.RegisterBinary("<", (a, b) => a < b ? 1 : 0, Precedence.Power);
            _context.RegisterUnary("!", (a) => a != 0 ? 0 : 1);
        }

        [Test]
        public void Evaluate_Boolean_Substitution()
        {
            MathExpr expr = new MathExpr("{a} and 1", _context);
            Assert.IsFalse(expr.Substitute("a", 0).EvalBoolean());
            Assert.IsTrue(expr.Substitute("a", 1).EvalBoolean());
        }

        [Test]
        public void Evaluate_Boolean_Numbers()
        {
            bool expr = "1 and 1".EvalBoolean(_context);
            Assert.IsTrue(expr);

            bool result = "1 and 0 or !0 and 3 > 2".EvalBoolean(_context);
            Assert.IsTrue(result);
        }

        [Test]
        public void Evaluate_Boolean_Globals()
        {
            Assert.IsTrue("{true} or {false}".EvalBoolean(_context));
            Assert.IsFalse("{false} or {false}".EvalBoolean(_context));
            Assert.IsFalse("{true} and {false}".EvalBoolean(_context));
            Assert.IsTrue("{true} and {true}".EvalBoolean(_context));
        }
    }

    static class BooleanMathExtensions
    {
        public static bool EvalBoolean(this string value) => value.ToMathExpr().EvalBoolean();

        public static bool EvalBoolean(this string value, IMathContext context) => value.ToMathExpr(context).EvalBoolean();

        public static bool EvalBoolean(this MathExpr value) => value.Result != 0;

        public static MathExpr Substitute(this MathExpr expr, string name, bool value)
            => expr.Substitute(name, value is true ? 1 : 0);
    }
}
