using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace StringMath.Benchmarks
{
    [MemoryDiagnoser(true)]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 0, iterationCount: 1, launchCount: 1)]
    [SimpleJob(RuntimeMoniker.Net48, warmupCount: 0, iterationCount: 1, launchCount: 1)]
    public class TokenizerBenchmarks
    {
        [Benchmark]
        public void Tokenize()
        {
            var tokenizer = new Tokenizer("1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}");

            Token token;

            do
            {
                token = tokenizer.ReadToken();
            }
            while (token.Type != TokenType.EndOfCode);
        }

        [Benchmark]
        public void Parse()
        {
            var tokenizer = new Tokenizer("1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}");
            var parser = new Parser(tokenizer, MathContext.Default);
            var expr = parser.Parse();
        }

        [Benchmark]
        public double Evaluate_Expr()
        {
            return "1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}".ToMathExpr().Substitute("b", 12989d).Substitute("ahghghh", 12345d).Result;
        }
    }
}
