using System;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a particular variable is the owner of an object
    /// </summary>
    public class Owns : Claim, IEquatable<Owns>, IExclusiveClaim
    {
        public new Variable Holder => (Variable)base.Holder;

        public Owns(Variable holder, Lifetime lifetime)
           : base(holder, lifetime)
        {
        }

        public override string ToString()
        {
            return $"{Holder} owns {Lifetime}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Owns);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Owns);
        }

        public bool Equals(Owns other)
        {
            return other != null &&
                   base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
