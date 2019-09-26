using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a given variable aliases an object. That is that it is one
    /// of possibly many references to the same object.
    /// </summary>
    public class Aliases : Claim, IEquatable<Aliases>, IShares
    {
        public Aliases(IClaimHolder holder, Lifetime lifetime)
            : base(holder, lifetime)
        {
        }

        public override string ToString()
        {
            return $"{Holder} aliases {Lifetime}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Aliases);
        }

        public override bool Equals(Claim? other)
        {
            return Equals(other as Aliases);
        }

        public bool Equals(Aliases? other)
        {
            return !(other is null) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }

        public static bool operator ==(Aliases alias1, Aliases alias2)
        {
            return EqualityComparer<Aliases>.Default.Equals(alias1, alias2);
        }

        public static bool operator !=(Aliases alias1, Aliases alias2)
        {
            return !(alias1 == alias2);
        }
    }
}
