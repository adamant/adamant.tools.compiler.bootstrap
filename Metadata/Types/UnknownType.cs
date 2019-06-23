namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class UnknownType : DataType
    {
        #region Singleton
        internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override bool IsKnown => false;

        /// <summary>
        /// Since an expression of unknown type can't actually be evaluated, its
        /// value semantics are "Empty". It doesn't ever produce a value.
        /// </summary>
        public override ValueSemantics ValueSemantics => ValueSemantics.Empty;

        public override string ToString()
        {
            return "⧼unknown⧽";
        }
    }
}
