using System;
using System.Globalization;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public struct Variable : /*IClaimHolder,*/ IEquatable<Variable>
    {
        public int Number { get; }
        public string Name => Number.ToString(CultureInfo.InvariantCulture);

        public Variable(int number)
        {
            Number = number;
        }

        public static readonly Variable Self = new Variable(0);

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

        public bool Equals(Variable other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Variable other && Number == other.Number;
        }
    }
}
