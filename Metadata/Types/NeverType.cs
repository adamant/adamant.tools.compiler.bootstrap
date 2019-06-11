namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class NeverType : SimpleType
    {
        #region Singleton
        internal static readonly NeverType Instance = new NeverType();

        private NeverType()
            : base("never")
        { }
        #endregion

        public override bool Exists => false;

        public override bool IsResolved => true;
    }
}
