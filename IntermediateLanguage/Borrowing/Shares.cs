using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a given variable shares an object. That is an immutable
    /// borrow.
    /// </summary>
    public class Shares : Claim, IEquatable<Shares>
    {
        public Shares(IClaimHolder holder, Lifetime lifetime)
            : base(holder, lifetime)
        {
        }

        public override string ToString()
        {
            return $"{Holder} shares {Lifetime}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Shares);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Shares);
        }

        public bool Equals(Shares other)
        {
            return other != null && base.Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }

        public static bool operator ==(Shares loan1, Shares loan2)
        {
            return EqualityComparer<Shares>.Default.Equals(loan1, loan2);
        }

        public static bool operator !=(Shares loan1, Shares loan2)
        {
            return !(loan1 == loan2);
        }
    }
}
