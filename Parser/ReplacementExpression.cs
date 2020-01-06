namespace SCLang
{
    internal class ReplacementExpression : Expression
    {
        public ReplacementExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
