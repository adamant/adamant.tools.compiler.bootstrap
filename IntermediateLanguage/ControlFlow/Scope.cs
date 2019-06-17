namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct Scope
    {
        private readonly int number;

        public static Scope Outer = new Scope(0);

        private Scope(int number)
        {
            this.number = number;
        }

        public Scope Next() => new Scope(number + 1);

        public override string ToString()
        {
            return "scope " + number;
        }

        public static bool operator ==(Scope a, Scope b)
        {
            return a.number == b.number;
        }

        public static bool operator !=(Scope a, Scope b)
        {
            return a.number != b.number;
        }

        public override int GetHashCode()
        {
            return number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Scope?;
            return this == other;
        }
    }
}
