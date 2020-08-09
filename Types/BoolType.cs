using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class BoolType : SimpleType
    {
        #region Singleton
        internal static readonly BoolType Instance = new BoolType();

        private BoolType()
            : base(SpecialTypeName.Bool)
        { }
        #endregion

        private protected BoolType(SpecialTypeName name)
            : base(name) { }

        public override bool IsKnown => true;
    }
}
