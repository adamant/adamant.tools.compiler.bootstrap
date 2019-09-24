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
    public struct Mutability
    {
        public static Mutability Mutable = new Mutability(true, Upgradable.None);

        /// <summary>
        /// An immutable reference that could be made mutable with an explicit `mut` expression.
        /// For example, a simple variable reference `x` to a variable declared with a mutable type.
        /// </summary>
        public static Mutability ExplicitlyUpgradable = new Mutability(false, Upgradable.Explicitly);

        /// <summary>
        /// A immutable reference that can be upgraded to an mutable one implicitly. This
        /// occurs with expressions returning ownership (i.e. `new`, `move` and
        /// functions returning `$owned`)
        /// </summary>
        public static Mutability ImplicitlyUpgradable = new Mutability(false, Upgradable.Implicitly);

        public static Mutability Immutable = new Mutability(false, Upgradable.None);

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
                case Upgradable.None:
                    return "";
                case Upgradable.Implicitly:
                case Upgradable.Explicitly:
                    return "(mut) ";
                default:
                    throw ExhaustiveMatch.Failed(upgradable);
            }
        }

        public static bool operator ==(Mutability left, Mutability right)
        {
            return left.mutable == right.mutable && left.upgradable == right.upgradable;
        }

        public static bool operator !=(Mutability left, Mutability right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(mutable, upgradable);
        }

        public override bool Equals(object obj)
        {
            var right = obj as Mutability?;
            return this == right;
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
