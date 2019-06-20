namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class UnknownType : DataType
    {
        #region Singleton
        internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override bool IsKnown => false;

        public override string ToString()
        {
            return "⧼unknown⧽";
        }
    }
}
