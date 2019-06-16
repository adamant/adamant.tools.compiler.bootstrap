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
    }
}
