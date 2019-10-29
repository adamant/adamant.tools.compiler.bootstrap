using System;
using System.Globalization;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct Variable : IClaimHolder, IEquatable<Variable>
    {
        public int Number { get; }
        public string Name => Number == 0 ? "result" : Number.ToString(CultureInfo.InvariantCulture);

        public Variable(int number)
        {
            Number = number;
        }

        public static readonly Variable Result = new Variable(0);

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
