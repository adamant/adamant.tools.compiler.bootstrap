namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct BasicBlockName
    {
        public readonly int Number;

        public BasicBlockName(int number)
        {
            Number = number;
        }

        public bool Equals(BasicBlockName other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is BasicBlockName other && Equals(other);
        }

        public static bool operator ==(BasicBlockName a, BasicBlockName b)
        {
            return a.Number == b.Number;
        }

        public static bool operator !=(BasicBlockName a, BasicBlockName b)
        {
            return a.Number != b.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return "bb" + Number;
        }
    }
}
