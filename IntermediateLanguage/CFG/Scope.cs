using System;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public struct Scope : IEquatable<Scope>
    {
        private readonly int number;

        public static readonly Scope Outer = new Scope(0);

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
            return HashCode.Combine(number);
        }

        public bool Equals(Scope other)
        {
            return this.number == other.number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Scope other && number == other.number;
        }
    }
}
