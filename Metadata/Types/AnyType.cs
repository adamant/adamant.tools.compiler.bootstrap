using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class AnyType : ReferenceType
    {
        #region Singleton
        internal static readonly AnyType Instance = new AnyType();

        private AnyType()
            : base(Lifetimes.Lifetime.Forever)
        { }
        #endregion

        public override bool IsResolved => true;

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Any";
    }
}
