using System;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public class SelfParameterReference : Reference
    {
        #region Singleton
        public static readonly SelfParameterReference Instance = new SelfParameterReference();

        private SelfParameterReference() { }
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
