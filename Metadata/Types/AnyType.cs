using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class AnyType : ReferenceType
    {
        #region Singleton
        internal static readonly AnyType ImmutableInstance = new AnyType(false);
        internal static readonly AnyType MutableInstance = new AnyType(false);

        private AnyType(bool mutable)
            : base(mutable, mutable ? Mutability.Mutable : Mutability.Immutable, Lifetime.Forever)
        { }
        #endregion

        public override bool IsKnown => true;

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Mutability == Mutability.Mutable ? "mut Any" : "Any";
    }
}
