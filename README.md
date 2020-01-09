# String Math
Calculates the value of a math expression from a string.
Supports variables and user defined operators.

```csharp
// Simple expression
Number result = Calculator.Evaluate("1 * (2 - 3) ^ 2"); // 1

// Variables
Number result = Calculator.Evaluate("{a} + 2 * {b}", new Replacement("a", 2), new Replacement("b", 1)); // 4

// Custom operators
Calculator.AddUnaryOperator("abs", a => a > 0 ? a : -a);
Calculator.AddBinaryOperator("max", (a, b) => a > b ? a : b);

// Using custom operators
Number result = Calculator.Evaluate("abs -5");  // 5
Number result = Calculator.Evaluate("3 max 4"); // 4
```
