namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// A reference to a variable
    /// </summary>
    public class RefType : DataType
    {
        public UserObjectType Referent { get; }
        public override bool IsKnown { get; }

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
