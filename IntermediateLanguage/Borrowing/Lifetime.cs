namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// The "name" of a lifetime of an object or function call.
    /// </summary>
    public struct Lifetime : IClaimHolder
    {
        public readonly int Number;

        public Lifetime(int number)
        {
            Number = number;
        }

        public override string ToString()
        {
            return "#" + Number;
        }

        public static bool operator ==(Lifetime a, Lifetime b)
        {
            return a.Number == b.Number;
        }

        public static bool operator !=(Lifetime a, Lifetime b)
        {
            return a.Number != b.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            var other = obj as Lifetime?;
            return this == other;
        }
    }
}
