namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The never or bottom type. That is a type with no values. A function
    /// with return type `never` must never return by normal means. It always
    /// either throws an exception, abandons the process or doesn't terminate.
    /// Because it is the bottom type, it is assignment compatible to all types
    /// this makes it particularly useful as the result type of expressions like
    /// `throw`, `return` and `break` which never produce a result. It is also
    /// used as the type of a `loop` statement with no breaks in it.
    /// </summary>
    public class NeverType : SimpleType
    {
        #region Singleton
        internal static readonly NeverType Instance = new NeverType();

        private NeverType()
            : base("never")
        { }
        #endregion

        public override bool IsEmpty => true;

        public override bool IsKnown => true;
    }
}
