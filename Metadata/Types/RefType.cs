namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// A reference to a variable
    /// </summary>
    public class RefType : DataType
    {
        public UserObjectType Referent { get; }
        public override bool IsKnown { get; }

        // TODO ref types could be borrow or alias based on mutability
        public override ValueSemantics ValueSemantics => ValueSemantics.Alias;

        public RefType(UserObjectType referent)
        {
            Referent = referent;
            IsKnown = referent.IsKnown;
        }

        public override string ToString()
        {
            return "ref " + Referent;
        }
    }
}
