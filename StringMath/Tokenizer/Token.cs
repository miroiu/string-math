namespace StringMath
{
    internal struct Token
    {
        public int Position;
        public string Text;
        public TokenType Type;

        public override string ToString()
        {
            return $"{Text} ({Type}):{Position}";
        }
    }
}