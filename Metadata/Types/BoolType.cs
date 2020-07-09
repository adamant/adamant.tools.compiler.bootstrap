namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public sealed class BoolType : SimpleType
    {
        #region Singleton
        internal static readonly BoolType Instance = new BoolType();

        private BoolType()
            : base("bool")
        { }
        #endregion

        public override bool IsKnown => true;
    }
}
