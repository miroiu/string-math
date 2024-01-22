using NUnit.Framework;
using System;

namespace StringMath.Tests
{
    [TestFixture]
    internal class MathExprTests
    {
        [SetUp]
        public void Setup()
        {
            MathExpr.AddOperator("abs", a => a > 0 ? a : -a);
            MathExpr.AddOperator("x", (a, b) => a * b);
            MathExpr.AddOperator("<<", (a, b) => a * Math.Pow(2, (int)b));
            MathExpr.AddOperator("<>", (a, b) => double.Parse($"{a}{b}"), Precedence.Prefix);
            MathExpr.AddOperator("e", (a, b) => double.Parse($"{a}e{b}"), Precedence.Power);
            MathExpr.AddOperator("sind", a => Math.Sin(a * (Math.PI / 180)));
        }

        [Test]
        public void Substitute_Should_Not_Overwrite_Global_Variable()
        {
            MathExpr.AddVariable("PI", Math.PI);
            MathExpr expr = "3 + {PI}";

            MathException ex = Assert.Throws<MathException>(() => expr.Substitute("PI", 1));
            Assert.AreEqual(ex.Code, MathException.ErrorCode.READONLY_VARIABLE);
        }

        [Test]
        public void Substitute_Should_Not_Set_Missing_Variable()
        {
            MathExpr expr = "3 + 2";

            MathException ex = Assert.Throws<MathException>(() => expr.Substitute("a", 1));
            Assert.AreEqual(ex.Code, MathException.ErrorCode.UNEXISTING_VARIABLE);
        }

        [Test]
        public void Substitute_Should_Set_Local_Variable()
        {
            MathExpr expr = "3 + {a}";
            expr.Substitute("a", 1);

            Assert.AreEqual(4, expr.Result);
        }

        [Test]
        public void Substitute_Should_Clear_Cache()
        {
            MathExpr expr = "{a} + 3";
            expr.Substitute("a", 3);

            Assert.AreEqual(6, expr.Result);

            expr.Substitute("a", 2);
            Assert.AreEqual(5, expr.Result);
        }

        [Test]
        public void Indexer_Should_Set_Local_Variable()
        {
            MathExpr expr = "3 + {a} + {b}";
            expr["a"] = 1;
            expr["b"] = 2;

            Assert.AreEqual(6, expr.Result);
        }

        [Test]
        public void Double_Conversion_Should_Evaluate_Expression()
        {
            double result = (MathExpr)"3 + 5";

            Assert.AreEqual(8, result);
        }

        [Test]
        [TestCase("{a}", new[] { "a" })]
        [TestCase("2 * {a} - {PI}", new[] { "a" })]
        [TestCase("({a} - 5) * 4 + {E}", new[] { "a" })]
        public void LocalVariables_Should_Exclude_Global_Variables(string input, string[] expected)
        {
            MathExpr expr = input;

            Assert.AreEqual(expected, expr.LocalVariables);
        }

        [Test]
        [TestCase("{a}", new[] { "a" })]
        [TestCase("2 * {a} - {PI}", new[] { "a", "PI" })]
        [TestCase("({a} - 5) * 4 + {E}", new[] { "a", "E" })]
        public void Variables_Should_Include_Global_Variables(string input, string[] expected)
        {
            MathExpr expr = input;

            Assert.AreEqual(expected, expr.Variables);
        }

        [Test]
        public void SetOperator_Binary_Should_Overwrite_Specified_Operator()
        {
            MathExpr.AddOperator("*", (x, y) => x * y);
            MathExpr expr = "5 * 3";
            expr.SetOperator("*", (x, y) => x);

            Assert.AreEqual(5, expr.Result);
        }

        [Test]
        public void SetOperator_Binary_Should_Not_Overwrite_Global_Operator()
        {
            MathExpr.AddOperator("*", (x, y) => x * y);
            MathExpr expr = "5 * 3";
            expr.SetOperator("*", (x, y) => x);

            MathExpr expr2 = "4 * 3";

            Assert.AreEqual(5, expr.Result);
            Assert.AreEqual(12, expr2.Result);
        }

        [Test]
        public void SetOperator_Unary_Should_Overwrite_Specified_Operator()
        {
            MathExpr.AddOperator(">>", (x) => x * x);
            MathExpr expr = ">> 5";
            expr.SetOperator(">>", (x) => x);

            Assert.AreEqual(5, expr.Result);
        }

        [Test]
        public void SetOperator_Unary_Should_Not_Overwrite_Global_Operator()
        {
            MathExpr.AddOperator("-", (x) => -x);
            MathExpr expr = "-5";
            expr.SetOperator("-", (x) => x);

            MathExpr expr2 = "-5";

            Assert.AreEqual(5, expr.Result);
            Assert.AreEqual(-5, expr2.Result);
        }

        [Test]
        [TestCase("1 +\t 2", 3)]
        [TestCase("-1.5 + 3", 1.5)]
        [TestCase("4!", 24)]
        [TestCase("(4 + 1)!", 120)]
        [TestCase("(3! + 1) * 2", 14)]
        [TestCase("2 ^ 3", 8)]
        [TestCase("1 + 16 log 2", 5)]
        [TestCase("1 + sqrt 4", 3)]
        [TestCase("sind(90) + sind 30", 1.5)]
        [TestCase("((1 + 1) + ((1 + 1) + (((1) + 1)) + 1))", 7)]
        public void Evaluate(string input, double expected)
        {
            double result = input.Eval();
            Assert.AreEqual(expected, result);
        }

        [TestCase("{b}+3*{a}", 3, 2, 11)]
        public void Evaluate(string input, double a, double b, double expected)
        {
            MathExpr expr = input;
            expr["a"] = a;
            expr["b"] = b;

            Assert.AreEqual(expected, expr.Result);
        }

        public void Evaluate_Using_GlobalVariables()
        {
            MathExpr.AddVariable("PI", Math.PI);
            MathExpr expr = "{PI}";

            Assert.AreEqual(Math.PI, expr.Result);
        }

        [Test]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("({a})", 3, 3)]
        public void Evaluate(string input, double variable, double expected)
        {
            MathExpr expr = input.Substitute("a", variable);

            Assert.AreEqual(expected, expr.Result);
        }

        [Test]
        [TestCase("abs -5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3 max 2", 3)]
        [TestCase("2 x\r\n 5", 10)]
        [TestCase("3 << 2", 12)]
        [TestCase("-3 <> 2", -32)]
        public void Evaluate_CustomOperators(string input, double expected)
        {
            MathExpr expr = input;
            Assert.AreEqual(expected, expr.Result);
        }

        [Test]
        [TestCase("abs -5 + {a}", 1, 6)]
        [TestCase("{a} + 2 * abs(-1) / {a}", 1, 3)]
        [TestCase("3 max {a}", 2, 3)]
        [TestCase("2 x\r\n {a}", 5, 10)]
        [TestCase("{a} << {a}", 3, 24)]
        [TestCase("-3 <> {a}", 2, -32)]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("({a})", 3, 3)]
        [TestCase("2 * ({a} + 3 + 5)", 1, 18)]
        public void Evaluate_With_Variables(string input, double variable, double expected)
        {
            MathExpr expr = input;
            expr.Substitute("a", variable);

            Assert.AreEqual(expr.Result, expected);
        }

        [Test]
        [TestCase("abs -5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3 max 2", 3)]
        [TestCase("2 x\r\n 5", 10)]
        [TestCase("3 << 2", 12)]
        [TestCase("-3 <> 2", -32)]
        public void Evaluate_CachedOperation_Without_Variables(string input, double expected)
        {
            MathExpr expr = input;

            Assert.AreEqual(expected, expr.Result);
            Assert.AreEqual(expected, expr.Result);
        }

        [Test]
        [TestCase("{a}+2")]
        public void Evaluate_Unassigned_Variable_Exception(string input)
        {
            MathExpr expr = input;

            MathException exception = Assert.Throws<MathException>(() => expr.Result.ToString());
            Assert.AreEqual(MathException.ErrorCode.UNASSIGNED_VARIABLE, exception.Code);
        }

        [Test]
        public void Evaluate_Sharing_Context()
        {
            MathExpr expr = "{a} + 1".Substitute("a", 2);
            expr.SetOperator("+", (a, b) => Math.Pow(a, b));

            Assert.AreEqual(2, expr.Result);

            MathExpr expr2 = "3 + 2".ToMathExpr(expr.Context);
            Assert.AreEqual(9, expr2.Result);

            double result = "1 + 2 + 3".Eval(expr.Context);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Evaluate_Custom_Context()
        {
            var context = new MathContext();
            context.RegisterBinary("+", (a, b) => Math.Pow(a, b));

            MathExpr expr = new MathExpr("{a} + 1", context).Substitute("a", 2);
            Assert.AreEqual(2, expr.Result);

            MathExpr expr2 = "3 + 2".ToMathExpr(context);
            Assert.AreEqual(9, expr2.Result);

            double result = "1 + 2 + 3".Eval(context);
            Assert.AreEqual(1, result);
        }

        [Test]
        [TestCase("1 + 5", 6)]
        [TestCase("1 + -5", -4)]
        [TestCase("2 * (abs(-5) + 1)", 12)]
        public void Compile(string input, double expected)
        {
            Func<double> fn = input.ToMathExpr().Compile();
            double result = fn();

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("1 + {a}", new[] { "a" }, new[] { 1d }, 2)]
        public void Compile_1Variable(string input, string[] paramsOrder, double[] paramsValues, double expected)
        {
            var fn = input.ToMathExpr().Compile(paramsOrder[0]);
            double result = fn(paramsValues[0]);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("(1 + {a}) * {b}", new[] { "b", "a" }, new[] { 2d, 3d }, 8)]
        [TestCase("(1 + {b}) * {a}", new[] { "b", "a" }, new[] { 2d, 3d }, 9)]
        public void Compile_2Variables(string input, string[] paramsOrder, double[] paramsValues, double expected)
        {
            var fn = input.ToMathExpr().Compile(paramsOrder[0], paramsOrder[1]);
            double result = fn(paramsValues[0], paramsValues[1]);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("({c} - 1 + {a}) * {b}", new[] { "b", "a", "c" }, new[] { 2d, 3d, 2d }, 8)]
        [TestCase("({c} - 1 + {b}) * {a}", new[] { "b", "a", "c" }, new[] { 2d, 3d, 2d }, 9)]
        public void Compile_3Variables(string input, string[] paramsOrder, double[] paramsValues, double expected)
        {
            var fn = input.ToMathExpr().Compile(paramsOrder[0], paramsOrder[1], paramsOrder[2]);
            double result = fn(paramsValues[0], paramsValues[1], paramsValues[2]);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Compile_Throws_Missing_Variable()
        {
            MathException ex = Assert.Throws<MathException>(() => "1 + {a}".ToMathExpr().Compile("b"));

            Assert.AreEqual(MathException.ErrorCode.UNEXISTING_VARIABLE, ex.Code);
        }

        [Test]
        public void Compile_Throws_Missing_Variable_When_No_Parameter_Provided()
        {
            MathException ex = Assert.Throws<MathException>(() => "1 + {a}".ToMathExpr().Compile());

            Assert.AreEqual(MathException.ErrorCode.UNEXISTING_VARIABLE, ex.Code);
        }

        [Test]
        public void Compile_Resolves_Remaining_Variables()
        {
            var expr = "1 + {a}".ToMathExpr().Substitute("a", 3);
            var fn = expr.Compile();
            double result = fn();

            Assert.AreEqual(4, result);
        }

        [Test]
        public void Compile_Resolves_Remaining_Variables2()
        {
            var expr = "1 + {a} * {b}".ToMathExpr().Substitute("a", 3);
            var fn = expr.Compile("b");
            double result = fn(2);

            Assert.AreEqual(7, result);
        }

        [Test]
        public void Compile_Resolves_Global_Variables()
        {
            var expr = "1 + {PI}".ToMathExpr();
            var fn = expr.Compile();
            double result = fn();

            Assert.AreEqual(1 + Math.PI, result);
        }
    }
}
