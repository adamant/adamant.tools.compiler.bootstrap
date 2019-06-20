namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class RefType : DataType
    {
        public ObjectType Referent { get; }
        public override bool IsKnown { get; }

        public RefType(ObjectType referent)
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
