using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct Variable : IClaimHolder
    {
        public readonly int Number;
        public string Name => Number == 0 ? "result" : Number.ToString();

        public Variable(int number)
        {
            Number = number;
        }

        public static Variable Result = new Variable(0);

        public override string ToString()
        {
            return "%" + Name;
        }

        public static bool operator ==(Variable a, Variable b)
        {
            return a.Number == b.Number;
        }

        public static bool operator !=(Variable a, Variable b)
        {
            return a.Number != b.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Variable?;
            return this == other;
        }
    }
}
