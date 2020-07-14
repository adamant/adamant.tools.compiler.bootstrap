namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class BoolType : SimpleType
    {
        #region Singleton
        internal static readonly BoolType Instance = new BoolType();

        private BoolType()
            : base("bool")
        { }
        #endregion

        private protected BoolType(string name)
            : base(name) { }

        public override bool IsKnown => true;
    }
}
