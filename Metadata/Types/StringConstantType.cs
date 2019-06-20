namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The type of string literals
    /// </summary>
    public class StringConstantType : SimpleType
    {
        #region Singleton
        internal static readonly StringConstantType Instance = new StringConstantType();

        private StringConstantType()
            : base("string")
        {
        }
        #endregion

        public override bool IsKnown => true;
    }
}
