using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a given variable has borrowed an object. That is, that it
    /// has temporarily claimed exclusive access to the object.
    /// </summary>
    public sealed class Borrows : Claim, IEquatable<Borrows>, IShares, IExclusiveClaim
    {
        public Borrows(IClaimHolder claimHolder, Lifetime lifetime)
            : base(claimHolder, lifetime)
        {
        }

        public override string ToString()
        {
            return $"{Holder} borrows {Lifetime}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Borrows);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Borrows);
        }

        public bool Equals(Borrows other)
        {
            return other != null && base.Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }

        public static bool operator ==(Borrows borrow1, Borrows borrow2)
        {
            return EqualityComparer<Borrows>.Default.Equals(borrow1, borrow2);
        }

        public static bool operator !=(Borrows borrow1, Borrows borrow2)
        {
            return !(borrow1 == borrow2);
        }
    }
}
