using System.Collections.Generic;

namespace StringMath
{
    public class OperationInfo
    {
        internal OperationInfo(Expression root, string expression, IReadOnlyCollection<string> replacements)
        {
            Root = root;
            Expression = expression;
            Replacements = replacements;
        }

        internal Expression Root { get; }
        public string Expression { get; }
        public IReadOnlyCollection<string> Replacements { get; }
    }
}
