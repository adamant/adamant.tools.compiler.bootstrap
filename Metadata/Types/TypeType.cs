using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class TypeType : ObjectType
    {
        #region Singleton
        internal static readonly TypeType Instance = new TypeType();

        private TypeType()
            : base(SpecialName.Type, false, Mutability.Immutable, Lifetime.None)
        { }
        #endregion

        public override bool IsKnown => true;

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Type";
    }
}
