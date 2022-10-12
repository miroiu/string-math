> V3 README can be found here: <https://github.com/miroiu/string-math/tree/release-3.0.0>

# String Math [![NuGet](https://img.shields.io/nuget/v/StringMath?style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath/) [![Downloads](https://img.shields.io/nuget/dt/StringMath?label=downloads&style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath) ![.NET](https://img.shields.io/static/v1?label=%20&message=Framework%204.6.1%20to%20NET%206&color=5C2D91&style=flat-square&logo=.net) ![](https://img.shields.io/static/v1?label=%20&message=documentation&color=yellow&style=flat-square)

Calculates the value of a math expression from a string returning a double.
Supports variables, user defined operators and expression compilation.

```csharp
double result = "1 * (2 - 3) ^ 2".Eval(); // 1
```

## Variables

```csharp
double result = "{a} + 2 * {b}".Substitute("a", 2).Substitute("b", 3).Result; // 8
```

### Global variables

These variables are inherited and cannot be substituted.

```csharp
MathExpr.AddVariable("PI", 3.1415926535897931);
double result = "1 + {PI}".Eval(); // 4.1415926535897931
```

## Custom operators

### Global operators

These operators are inherited and can be overidden.

```csharp
MathExpr.AddOperator("abs", a => a > 0 ? a : -a);
double result = "abs -5".Eval(); // 5

// Operator precedence (you can specify an int for precedence)
MathExpr.AddOperator("max", (a, b) => a > b ? a : b, Precedence.Power);
double result = new MathExpr("2 * 3 max 4").Result; // 8
```

### Local operators

These are applied only to the target expression.

```csharp
MathExpr expr = "{PI} + 1";
expr.SetOperator("+", (a, b) => Math.Pow(a, b));
double result = expr; // 3.1415926535897931

double result2 = "{PI} + 1".Eval(); // 4.1415926535897931
```

## Advanced

### Extract variables

```csharp
var expr = "{a} + {b} + {PI}".ToMathExpr();
var variables = expr.Variables; // { "a", "b", "PI" }
var localVariables = expr.LocalVariables; // { "a", "b" }
```

### Compilation

```csharp
Func<double, double> fn = "{a} + 2".ToMathExpr().Compile("a");
double result = fn(5); // 7
```

### Conditional substitution

```csharp
MathExpr expr = "1 / {a}".Substitute("a", 1);

double temp = expr.Result; // 1

if (someCondition)  // true
 expr.Substitute("a", 2);

double final = expr.Result; // 0.5
```

### Sharing math context

```csharp
MathExpr expr = "{PI} + 1";
expr.SetOperator("+", (a, b) => Math.Pow(a, b));

MathExpr expr2 = "3 + 2".ToMathExpr(expr.Context);

double result = "1 + 2 + 3".Eval(expr.Context);
```

### Custom math context

```csharp
var context = new MathContext(); // new MathContext(MathContext.Default); // to inherit from global
context.RegisterBinary("+", (a, b) => Math.Pow(a, b));

MathExpr expr = new MathExpr("{PI} + 1", context);
MathExpr expr2 = "3 + 2".ToMathExpr(context);
double result = "1 + 2 + 3".Eval(context);
```

## Default operators

### Binary

```csharp
+ (addition)
- (subtraction)
* (multiplication)
/ (division)
% (remainder)
^ (power)
log (logarithm)
max (maximum)
min (minimum)
```

### Unary

```csharp
- (negation)
! (factorial)
sqrt (square root)
sin (sine)
asin (arcsine)
cos (cosine)
acos (arccosine)
tan (tangent)
atan (arctangent)
rad (convert degrees to radians)
deg (convert radians to degrees)
ceil (ceiling)
floor (floor)
round (rounding)
exp (e raised to power)
abs (absolute)
```
