# String Math
Calculates the value of a math expression from a string returning a decimal.
Supports variables and user defined operators.

```csharp
// Evaluating a simple expression
Calculator myCalculator = new Calculator();
decimal result = myCalculator.Evaluate("1 * (2 - 3) ^ 2"); // 1

// Using custom operators
myCalculator.AddOperator("abs", a => a > 0 ? a : -a);
decimal result = myCalculator.Evaluate("abs -5"); // 5

// Using custom operator precedence (you can specify an int for precedence)
myCalculator.AddOperator("max", (a, b) => a > b ? a : b, Precedence.Power);
decimal result = myCalculator.Evaluate("2 * 3 max 4"); // 8
```

```csharp 
// Using a variables collection
Replacements variables = new Replacements
{
	["a"] = 5,
	["PI"] = 3.1415926535897931
};
variables["a"] = 0;

Calculator myCalculator = new Calculator(variables);

// Replacing existing variables
myCalculator.Replace("a", 2);
myCalculator.Replace("b", 1);
decimal result = calculator.Evaluate("{a} + 2 * {b} + {PI}"); // 7.1415926535897931
```

```csharp
// Using the static class SMath
decimal result = SMath.Evaluate("1 + 1"); // 2
decimal result = SMath.Evaluate("1 + {myVar}", new Replacements { ["myVar"] = 1 }); // 2
```
