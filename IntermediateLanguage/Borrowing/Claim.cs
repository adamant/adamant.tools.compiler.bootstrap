using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    public abstract class Claim : IEquatable<Claim>
    {
        public IClaimHolder Holder { get; }
        public Lifetime Lifetime { get; }

        protected Claim(IClaimHolder holder, Lifetime lifetime)
        {
            Holder = holder;
            Lifetime = lifetime;
        }

        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            return Equals(obj as Claim);
        }

        public virtual bool Equals(Claim other)
        {
            return other != null &&
                   Holder == other.Holder &&
                   Lifetime == other.Lifetime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Holder, Lifetime);
        }

        public static bool operator ==(Claim claim1, Claim claim2)
        {
            return EqualityComparer<Claim>.Default.Equals(claim1, claim2);
        }

        public static bool operator !=(Claim claim1, Claim claim2)
        {
            return !(claim1 == claim2);
        }
    }
}
