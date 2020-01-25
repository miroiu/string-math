# String Math
Calculates the value of a math expression from a string.
Supports variables and user defined operators.

```csharp
// Simple expression
decimal result = Calculator.Evaluate("1 * (2 - 3) ^ 2"); // 1

// Variables
decimal result = Calculator.Evaluate("{a} + 2 * {b}", new Replacement("a", 2), new Replacement("b", 1)); // 4

// Adding custom operators
Calculator.AddOperator("abs", a => a > 0 ? a : -a);
Calculator.AddOperator("max", (a, b) => a > b ? a : b);

// Using custom operators
decimal result = Calculator.Evaluate("abs -5"); // 5
decimal result = Calculator.Evaluate("3 max 4"); // 4
```
