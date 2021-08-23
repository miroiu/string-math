using System.Collections.Generic;

namespace StringMath
{
    public class OperationInfo
    {
        internal OperationInfo(Expression root, string expression, IReadOnlyCollection<string> variables)
        {
            Root = root;
            Expression = expression;
            Variables = variables;
        }

        internal Expression Root { get; }
        public string Expression { get; }
        public IReadOnlyCollection<string> Variables { get; }
    }
}
