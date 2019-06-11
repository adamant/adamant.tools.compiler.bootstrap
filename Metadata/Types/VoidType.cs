namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class VoidType : SimpleType
    {
        #region Singleton
        internal static readonly VoidType Instance = new VoidType();

        private VoidType()
            : base("void")
        { }
        #endregion

        public override bool Exists => false;

        public override bool IsResolved => true;
    }
}
