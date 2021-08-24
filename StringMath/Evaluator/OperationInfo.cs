using System.Collections.Generic;

namespace StringMath
{
    /// <summary>An optimized and cached math expression.</summary>
    public class OperationInfo
    {
        internal OperationInfo(Expression root, string expression, IReadOnlyCollection<string> variables)
        {
            Root = root;
            Expression = expression;
            Variables = variables;
        }

        internal Expression Root { get; }

        /// <summary>The math expression that was used to create this operation.</summary>
        public string Expression { get; }

        /// <summary>A collection of variable names extracted from the <see cref="Expression"/>.</summary>
        public IReadOnlyCollection<string> Variables { get; }
    }
}
