using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class AnyType : ReferenceType
    {
        #region Singleton
        internal static readonly AnyType ImmutableInstance = new AnyType(false);
        internal static readonly AnyType MutableInstance = new AnyType(false);

        private AnyType(bool mutable)
            : base(mutable, mutable ? Mutability.Mutable : Mutability.Immutable,
                mutable ? ReferenceCapability.Borrowed : ReferenceCapability.Shared)
        { }
        #endregion

        public override bool IsKnown => true;

        protected internal override Self AsImmutableReturnsSelf()
        {
            throw new System.NotImplementedException();
        }

        //protected internal override Self WithLifetimeReturnsSelf(Lifetime lifetime)
        //{
        //    throw new System.NotImplementedException();
        //}

        public override string ToString() => Mutability == Mutability.Mutable ? "mut Any" : "Any";
    }
}
