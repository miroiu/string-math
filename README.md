# String Math [![NuGet](https://img.shields.io/nuget/v/StringMath?style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath/) [![Downloads](https://img.shields.io/nuget/dt/StringMath?label=downloads&style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath) ![.NET](https://img.shields.io/static/v1?label=%20&message=standard%202.0&color=5C2D91&style=flat-square&logo=.net) ![](https://img.shields.io/static/v1?label=%20&message=documentation&color=yellow&style=flat-square)
Calculates the value of a math expression from a string returning a double.
Supports variables and user defined operators.

### Creating and using a calculator
```csharp
ICalculator myCalculator = new Calculator();
double result = myCalculator.Evaluate("1 * (2 - 3) ^ 2"); // 1

// Using custom operators
myCalculator.AddOperator("abs", a => a > 0 ? a : -a);
double result = myCalculator.Evaluate("abs -5"); // 5

// Using custom operator precedence (you can specify an int for precedence)
myCalculator.AddOperator("max", (a, b) => a > b ? a : b, Precedence.Power);
double result = myCalculator.Evaluate("2 * 3 max 4"); // 8
```

### Creating and using variables
```csharp
// Default variables collection is optional
ICalculator myCalculator = new Calculator(new VariablesCollection
{
	["a"] = 5,
	["PI"] = 3.1415926535897931
});

// Setting or creating variables
myCalculator.SetValue("a", 2);
myCalculator["b"] = 1;

double result = myCalculator.Evaluate("{a} + 2 * {b} + {PI}"); // 7.1415926535897931
```

### Creating and reusing operations
```csharp
myCalculator.SetValue("a", 5);

// Creating operations (expression tree is optimized and cached)
OperationInfo op = myCalculator.CreateOperation("2 * {a}");
double result1 = myCalculator.Evaluate(op); // 10

myCalculator["a"] = 3;
// Reusing the operation (improved performance)
double result2 = myCalculator.Evaluate(op); // 6
```

### Using the static api: SMath
```csharp
// Same API as a calculator instance
double result = SMath.Evaluate("1 + 1"); // 2
SMath.SetValue("myVar", 1);
double result = SMath.Evaluate("1 + {myVar}", ); // 2
```

#### Default binary operators
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

#### Default unary operators
```csharp
- (negation)
! (factorial)
sqrt (square root)
sin (sinus)
cos (cosinus)
tan (tangent)
ceil (ceiling)
floor (floor)
round (rounding)
exp (e raised to power)
abs (absolute)
```
