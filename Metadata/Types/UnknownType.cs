namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The type of expressions and values whose type could not be determined or
    /// was somehow invalid. The unknown type can't be directly used in code.
    /// No well typed program contains any value of the unknown type.
    /// </summary>
    public sealed class UnknownType : DataType
    {
        #region Singleton
        internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override bool IsKnown => false;

        /// <summary>
        /// Like `never` values of type unknown can be assigned to any value.
        /// It acts like a bottom type in this respect.
        /// </summary>
        public override ValueSemantics ValueSemantics => ValueSemantics.Never;

        /// <summary>
        /// Since an expression of unknown type can't actually be evaluated, its
        /// value semantics are "Empty". It doesn't ever produce a value.
        /// </summary>
        public override OldValueSemantics OldValueSemantics => OldValueSemantics.Empty;

        public override string ToString()
        {
            return "⧼unknown⧽";
        }
    }
}
