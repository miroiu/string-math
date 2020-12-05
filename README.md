# String Math [![NuGet](https://img.shields.io/nuget/v/StringMath?style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath/) [![Downloads](https://img.shields.io/nuget/dt/StringMath?label=downloads&style=flat-square&logo=nuget)](https://www.nuget.org/packages/StringMath) ![.NET](https://img.shields.io/static/v1?label=%20&message=standard%202.0&color=5C2D91&style=flat-square&logo=.net)
Calculates the value of a math expression from a string returning a decimal.
Supports variables and user defined operators.

### Creating and using a calculator
```csharp
Calculator myCalculator = new Calculator();
decimal result = myCalculator.Evaluate("1 * (2 - 3) ^ 2"); // 1

// Using custom operators
myCalculator.AddOperator("abs", a => a > 0 ? a : -a);
decimal result = myCalculator.Evaluate("abs -5"); // 5

// Using custom operator precedence (you can specify an int for precedence)
myCalculator.AddOperator("max", (a, b) => a > b ? a : b, Precedence.Power);
decimal result = myCalculator.Evaluate("2 * 3 max 4"); // 8
```

### Creating and using variables
```csharp
// Creating a variables collection
Replacements variables = new Replacements
{
	["a"] = 5,
	["PI"] = 3.1415926535897931
};

Calculator myCalculator = new Calculator(variables); // default variables collection is optional (can still add variables without a collection)

// Replacing or creating variables
myCalculator.Replace("a", 2);
myCalculator.Replace("b", 1); // new syntax: myCalculator["b"] = 1;
decimal result = myCalculator.Evaluate("{a} + 2 * {b} + {PI}"); // 7.1415926535897931
```

### Creating and reusing operations
```csharp
myCalculator["a"] = 5;

// Creating operations
OperationInfo op = myCalculator.CreateOperation("2 * {a}");
decimal result1 = myCalculator.Evaluate(op); // 10

// Reusing the operation (improves performance)
myCalculator["a"] = 3;
decimal result2 = myCalculator.Evaluate(op); // 6
```

### Using the static api: SMath
```csharp
// Same API as a calculator instance except the Evaluate method
decimal result = SMath.Evaluate("1 + 1"); // 2
decimal result = SMath.Evaluate("1 + {myVar}", new Replacements { ["myVar"] = 1 }); // 2
```
