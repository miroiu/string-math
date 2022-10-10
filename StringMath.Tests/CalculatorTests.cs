using NUnit.Framework;
using System;

namespace StringMath.Tests
{
    [TestFixture]
    internal class CalculatorTests
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
        public void Variables_Should_Exclude_Global_Variables()
        {
            MathExpr.AddVariable("PI", Math.PI);
            MathExpr expr = "3 + {a} + {PI}";

            Assert.AreEqual(new string[] { "a" }, expr.Variables);
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
        [TestCase("(3! + 1) * 2", 14)]
        [TestCase("2 ^ 3", 8)]
        [TestCase("1 + 16 log 2", 5)]
        [TestCase("1 + sqrt 4", 3)]
        [TestCase("sind(90) + sind 30", 1.5)]
        [TestCase("((1 + 1) + ((1 + 1) + (((1) + 1)) + 1))", 7)]
        public void Evaluate(string input, double expected)
        {
            MathExpr expr = input;
            Assert.AreEqual(expected, expr.Result);
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
            MathExpr expr = input;
            expr["a"] = variable;

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

            Assert.AreEqual(input, expr.Text);

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
    }
}
