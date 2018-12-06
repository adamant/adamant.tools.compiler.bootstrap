namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class RefType : DataType
    {
        public ObjectType Referent { get; }
        public override bool IsResolved { get; }

        public RefType(ObjectType referent)
        {
            Referent = referent;
            IsResolved = referent.IsResolved;
        }

        public override string ToString()
        {
            return "ref " + Referent;
        }
    }
}
