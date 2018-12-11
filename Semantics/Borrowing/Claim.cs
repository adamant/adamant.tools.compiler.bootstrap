using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public abstract class Claim : IEquatable<Claim>
    {
        public int Variable { get; }
        public int ObjectId { get; }

        protected Claim(int variable, int objectId)
        {
            Variable = variable;
            ObjectId = objectId;
        }

        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            return Equals(obj as Claim);
        }

        public virtual bool Equals(Claim other)
        {
            return other != null &&
                   Variable == other.Variable &&
                   ObjectId == other.ObjectId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Variable, ObjectId);
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
