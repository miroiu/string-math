using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    /// <remarks>Inherits operators from <see cref="Parent"/>.</remarks>
    public sealed class MathContext : IMathContext
    {
        private readonly Dictionary<string, Func<double, double, double>> _binaryEvaluators = new Dictionary<string, Func<double, double, double>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Func<double, double>> _unaryEvaluators = new Dictionary<string, Func<double, double>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Precedence> _binaryPrecedence = new Dictionary<string, Precedence>(StringComparer.Ordinal);
        private readonly HashSet<string> _operators = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>The global instance used by <see cref="MathExpr.AddOperator(string, Func{double, double})"/> methods.</summary>
        public static readonly IMathContext Default = new MathContext();

        /// <summary>The parent context to inherit from.</summary>
        public IMathContext? Parent { get; }

        static MathContext()
        {
            var rad = Math.PI / 180;
            var deg = 180 / Math.PI;

            Default.RegisterBinary("+", (a, b) => a + b, Precedence.Addition);
            Default.RegisterBinary("-", (a, b) => a - b, Precedence.Addition);
            Default.RegisterBinary("*", (a, b) => a * b, Precedence.Multiplication);
            Default.RegisterBinary("/", (a, b) => a / b, Precedence.Multiplication);
            Default.RegisterBinary("%", (a, b) => a % b, Precedence.Multiplication);
            Default.RegisterBinary("^", (a, b) => Math.Pow(a, b), Precedence.Power);
            Default.RegisterBinary("log", (a, b) => Math.Log(a, b), Precedence.Logarithmic);
            Default.RegisterBinary("max", (a, b) => Math.Max(a, b), Precedence.UserDefined);
            Default.RegisterBinary("min", (a, b) => Math.Min(a, b), Precedence.UserDefined);

            Default.RegisterUnary("-", a => -a);
            Default.RegisterUnary("!", a => ComputeFactorial(a));
            Default.RegisterUnary("sqrt", a => Math.Sqrt(a));
            Default.RegisterUnary("sin", a => Math.Sin(a));
            Default.RegisterUnary("asin", a => Math.Asin(a));
            Default.RegisterUnary("cos", a => Math.Cos(a));
            Default.RegisterUnary("acos", a => Math.Acos(a));
            Default.RegisterUnary("tan", a => Math.Tan(a));
            Default.RegisterUnary("atan", a => Math.Atan(a));
            Default.RegisterUnary("ceil", a => Math.Ceiling(a));
            Default.RegisterUnary("floor", a => Math.Floor(a));
            Default.RegisterUnary("round", a => Math.Round(a));
            Default.RegisterUnary("exp", a => Math.Exp(a));
            Default.RegisterUnary("abs", a => Math.Abs(a));
            Default.RegisterUnary("rad", a => rad * a);
            Default.RegisterUnary("deg", a => deg * a);
        }

        /// <summary>Creates a new instance of a MathContext.</summary>
        /// <param name="parent">The parent context to inherit operators from.</param>
        public MathContext(IMathContext parent)
            => Parent = parent;

        /// <summary>Creates a new instance of a MathContext.</summary>
        public MathContext() { }

        /// <inheritdoc />
        public bool IsUnary(string operatorName)
            => _unaryEvaluators.ContainsKey(operatorName) || (Parent?.IsUnary(operatorName) ?? false);

        /// <inheritdoc />
        public bool IsBinary(string operatorName)
            => _binaryEvaluators.ContainsKey(operatorName) || (Parent?.IsBinary(operatorName) ?? false);

        /// <inheritdoc />
        public Precedence GetBinaryPrecedence(string operatorName)
        {
            return _binaryPrecedence.ContainsKey(operatorName)
                ? _binaryPrecedence[operatorName]
                : Parent?.GetBinaryPrecedence(operatorName)
                ?? throw MathException.MissingBinaryOperator(operatorName);
        }

        /// <inheritdoc />
        public void RegisterBinary(string operatorName, Func<double, double, double> operation, Precedence? precedence = default)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _binaryEvaluators[operatorName] = operation;
            _binaryPrecedence[operatorName] = precedence ?? Precedence.UserDefined;
            _operators.Add(operatorName);
        }

        /// <inheritdoc />
        public void RegisterUnary(string operatorName, Func<double, double> operation)
        {
            operatorName.EnsureNotNull(nameof(operatorName));
            operation.EnsureNotNull(nameof(operation));

            _unaryEvaluators[operatorName] = operation;
            _operators.Add(operatorName);
        }

        /// <inheritdoc />
        public double EvaluateBinary(string op, double a, double b)
        {
            double result = _binaryEvaluators.ContainsKey(op)
                ? _binaryEvaluators[op](a, b)
                : Parent?.EvaluateBinary(op, a, b)
                ?? throw MathException.MissingBinaryOperator(op);

            return result;
        }

        /// <inheritdoc />
        public double EvaluateUnary(string op, double a)
        {
            double result = _unaryEvaluators.ContainsKey(op)
                ? _unaryEvaluators[op](a)
                : Parent?.EvaluateUnary(op, a)
                ?? throw MathException.MissingUnaryOperator(op);

            return result;
        }

        #region Factorial

        private static readonly Dictionary<double, double> _factorials = new Dictionary<double, double>
        {
            [0] = 1d,
            [1] = 1d,
            [2] = 2d,
            [3] = 6d,
            [4] = 24d,
            [5] = 120d,
            [6] = 720d,
            [7] = 5040d,
            [8] = 40320d,
            [9] = 362880d,
            [10] = 3628800d,
            [11] = 39916800d,
            [12] = 479001600d,
            [13] = 6227020800d,
            [14] = 87178291200d,
            [15] = 1307674368000d,
            [16] = 20922789888000d,
            [17] = 355687428096000d,
            [18] = 6402373705728000d,
            [19] = 1.21645100408832E+17d,
            [20] = 2.43290200817664E+18d,
            [21] = 5.109094217170944E+19d,
            [22] = 1.1240007277776077E+21d,
            [23] = 2.585201673888498E+22d,
            [24] = 6.204484017332394E+23d,
            [25] = 1.5511210043330986E+25d,
            [26] = 4.0329146112660565E+26d,
            [27] = 1.0888869450418352E+28d,
            [28] = 3.0488834461171384E+29d,
            [29] = 8.841761993739701E+30d,
            [30] = 2.6525285981219103E+32d,
            [31] = 8.222838654177922E+33d,
            [32] = 2.631308369336935E+35d,
            [33] = 8.683317618811886E+36d,
            [34] = 2.9523279903960412E+38d,
            [35] = 1.0333147966386144E+40d,
            [36] = 3.719933267899012E+41d,
            [37] = 1.3763753091226343E+43d,
            [38] = 5.23022617466601E+44d,
            [39] = 2.0397882081197442E+46d,
            [40] = 8.159152832478977E+47d,
            [41] = 3.3452526613163803E+49d,
            [42] = 1.4050061177528798E+51d,
            [43] = 6.041526306337383E+52d,
            [44] = 2.6582715747884485E+54d,
            [45] = 1.1962222086548019E+56d,
            [46] = 5.5026221598120885E+57d,
            [47] = 2.5862324151116818E+59d,
            [48] = 1.2413915592536073E+61d,
            [49] = 6.082818640342675E+62d,
            [50] = 3.0414093201713376E+64d,
            [51] = 1.5511187532873822E+66d,
            [52] = 8.065817517094388E+67d,
            [53] = 4.2748832840600255E+69d,
            [54] = 2.308436973392414E+71d,
            [55] = 1.2696403353658276E+73d,
            [56] = 7.109985878048635E+74d,
            [57] = 4.052691950487722E+76d,
            [58] = 2.350561331282879E+78d,
            [59] = 1.3868311854568986E+80d,
            [60] = 8.320987112741392E+81d,
            [61] = 5.075802138772248E+83d,
            [62] = 3.146997326038794E+85d,
            [63] = 1.98260831540444E+87d,
            [64] = 1.2688693218588417E+89d,
            [65] = 8.247650592082472E+90d,
            [66] = 5.443449390774431E+92d,
            [67] = 3.647111091818868E+94d,
            [68] = 2.4800355424368305E+96d,
            [69] = 1.711224524281413E+98d,
            [70] = 1.197857166996989E+100d,
            [71] = 8.504785885678622E+101d,
            [72] = 6.123445837688608E+103d,
            [73] = 4.4701154615126834E+105d,
            [74] = 3.3078854415193856E+107d,
            [75] = 2.480914081139539E+109d,
            [76] = 1.8854947016660498E+111d,
            [77] = 1.4518309202828584E+113d,
            [78] = 1.1324281178206295E+115d,
            [79] = 8.946182130782973E+116d,
            [80] = 7.156945704626378E+118d,
            [81] = 5.797126020747366E+120d,
            [82] = 4.75364333701284E+122d,
            [83] = 3.945523969720657E+124d,
            [84] = 3.314240134565352E+126d,
            [85] = 2.8171041143805494E+128d,
            [86] = 2.4227095383672724E+130d,
            [87] = 2.107757298379527E+132d,
            [88] = 1.8548264225739836E+134d,
            [89] = 1.6507955160908452E+136d,
            [90] = 1.4857159644817607E+138d,
            [91] = 1.3520015276784023E+140d,
            [92] = 1.24384140546413E+142d,
            [93] = 1.1567725070816409E+144d,
            [94] = 1.0873661566567424E+146d,
            [95] = 1.0329978488239052E+148d,
            [96] = 9.916779348709491E+149d,
            [97] = 9.619275968248206E+151d,
            [98] = 9.426890448883242E+153d,
            [99] = 9.33262154439441E+155d,
            [100] = 9.33262154439441E+157d,
            [101] = 9.425947759838354E+159d,
            [102] = 9.614466715035121E+161d,
            [103] = 9.902900716486175E+163d,
            [104] = 1.0299016745145622E+166d,
            [105] = 1.0813967582402903E+168d,
            [106] = 1.1462805637347078E+170d,
            [107] = 1.2265202031961373E+172d,
            [108] = 1.3246418194518284E+174d,
            [109] = 1.4438595832024928E+176d,
            [110] = 1.5882455415227421E+178d,
            [111] = 1.7629525510902437E+180d,
            [112] = 1.9745068572210728E+182d,
            [113] = 2.2311927486598123E+184d,
            [114] = 2.543559733472186E+186d,
            [115] = 2.925093693493014E+188d,
            [116] = 3.3931086844518965E+190d,
            [117] = 3.969937160808719E+192d,
            [118] = 4.6845258497542883E+194d,
            [119] = 5.574585761207603E+196d,
            [120] = 6.689502913449124E+198d,
            [121] = 8.09429852527344E+200d,
            [122] = 9.875044200833598E+202d,
            [123] = 1.2146304367025325E+205d,
            [124] = 1.5061417415111404E+207d,
            [125] = 1.8826771768889254E+209d,
            [126] = 2.372173242880046E+211d,
            [127] = 3.012660018457658E+213d,
            [128] = 3.8562048236258025E+215d,
            [129] = 4.9745042224772855E+217d,
            [130] = 6.466855489220472E+219d,
            [131] = 8.471580690878817E+221d,
            [132] = 1.118248651196004E+224d,
            [133] = 1.4872707060906852E+226d,
            [134] = 1.992942746161518E+228d,
            [135] = 2.6904727073180495E+230d,
            [136] = 3.659042881952547E+232d,
            [137] = 5.01288874827499E+234d,
            [138] = 6.917786472619486E+236d,
            [139] = 9.615723196941086E+238d,
            [140] = 1.346201247571752E+241d,
            [141] = 1.89814375907617E+243d,
            [142] = 2.6953641378881614E+245d,
            [143] = 3.8543707171800706E+247d,
            [144] = 5.550293832739301E+249d,
            [145] = 8.047926057471987E+251d,
            [146] = 1.17499720439091E+254d,
            [147] = 1.7272458904546376E+256d,
            [148] = 2.5563239178728637E+258d,
            [149] = 3.808922637630567E+260d,
            [150] = 5.7133839564458505E+262d,
            [151] = 8.627209774233235E+264d,
            [152] = 1.3113358856834518E+267d,
            [153] = 2.006343905095681E+269d,
            [154] = 3.089769613847349E+271d,
            [155] = 4.789142901463391E+273d,
            [156] = 7.47106292628289E+275d,
            [157] = 1.1729568794264138E+278d,
            [158] = 1.8532718694937338E+280d,
            [159] = 2.946702272495037E+282d,
            [160] = 4.714723635992059E+284d,
            [161] = 7.590705053947215E+286d,
            [162] = 1.2296942187394488E+289d,
            [163] = 2.0044015765453015E+291d,
            [164] = 3.2872185855342945E+293d,
            [165] = 5.423910666131586E+295d,
            [166] = 9.003691705778433E+297d,
            [167] = 1.5036165148649983E+300d,
            [168] = 2.526075744973197E+302d,
            [169] = 4.2690680090047027E+304d,
            [170] = 7.257415615307994E+306d,
        };

        private static double ComputeFactorial(double value)
        {
            if (value > 170)
            {
                throw new InvalidOperationException("Result is too big.");
            }

            if (value < 0)
            {
                throw new ArgumentException("Value cannot be negative.", nameof(value));
            }

            return _factorials[value];
        }

        #endregion
    }
}
