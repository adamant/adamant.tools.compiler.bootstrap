namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class UnknownType : DataType
    {
        #region Singleton
        public static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override string ToString()
        {
            return "unknown";
        }
    }
}
