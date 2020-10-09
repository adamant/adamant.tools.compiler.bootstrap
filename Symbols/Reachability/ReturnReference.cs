using System;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public class ReturnReference : Reference
    {
        #region Singleton

        public static readonly ReturnReference Instance = new ReturnReference();

        private ReturnReference() { }

        #endregion

        public override bool Equals(Reference? other)
        {
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(typeof(SelfParameterReference));
        }
    }
}
