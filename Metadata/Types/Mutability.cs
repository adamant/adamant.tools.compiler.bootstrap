using System;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    internal enum Upgradable
    {
        None = 0,
        Explicitly,
        Implicitly
    }

    [DebuggerDisplay("{" + nameof(Name) + ",nq}")]
    public struct Mutability : IEquatable<Mutability>
    {
        public static readonly Mutability Mutable = new Mutability(true, Upgradable.None);

        /// <summary>
        /// An immutable reference that could be made mutable with an explicit `mut` expression.
        /// For example, a simple variable reference `x` to a variable declared with a mutable type.
        /// </summary>
        public static readonly Mutability ExplicitlyUpgradable = new Mutability(false, Upgradable.Explicitly);

        /// <summary>
        /// A immutable reference that can be upgraded to an mutable one implicitly. This
        /// occurs with expressions returning ownership (i.e. `new`, `move` and
        /// functions returning `$owned`)
        /// </summary>
        public static readonly Mutability ImplicitlyUpgradable = new Mutability(false, Upgradable.Implicitly);

        public static readonly Mutability Immutable = new Mutability(false, Upgradable.None);

        /// <summary>
        /// Whether the reference itself is mutable
        /// </summary>
        private readonly bool mutable;

        /// <summary>
        /// Whether the mutability is fixed or indeterminate and hence convertible to the other mutability
        /// </summary>
        private readonly Upgradable upgradable;

        public bool IsUpgradable => upgradable != Upgradable.None;

        private Mutability(bool mutable, Upgradable upgradable)
        {
            this.mutable = mutable;
            this.upgradable = upgradable;
        }

        public bool IsAssignableFrom(Mutability other, bool allowExplicitUpgrade = false)
        {
            return !mutable
                   || other.mutable
                   || other.upgradable == Upgradable.Implicitly
                   || (allowExplicitUpgrade && other.upgradable == Upgradable.Explicitly);
        }

        public override string ToString()
        {
            if (mutable)
                return "mut ";

            switch (upgradable)
            {
                default:
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                    throw ExhaustiveMatch.Failed(upgradable);
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                case Upgradable.None:
                    return "";
                case Upgradable.Implicitly:
                case Upgradable.Explicitly:
                    return "(mut) ";
            }
        }

        public static bool operator ==(Mutability left, Mutability right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Mutability left, Mutability right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(mutable, upgradable);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Mutability other) return Equals(other);
            return false;
        }

        public bool Equals(Mutability other)
        {
            return mutable == other.mutable && upgradable == other.upgradable;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Name
        {
            get
            {
                if (mutable)
                    return "Mutable";

                switch (upgradable)
                {
                    case Upgradable.None:
                        return "Immutable";
                    case Upgradable.Implicitly:
                        return "Immutable (Implicitly Upgrade)";
                    case Upgradable.Explicitly:
                        return "Immutable (Explicitly Upgrade) ";
                    default:
                        throw ExhaustiveMatch.Failed(upgradable);
                }
            }
        }
    }
}
