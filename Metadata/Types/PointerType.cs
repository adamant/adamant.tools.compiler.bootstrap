namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class PointerType : DataType
    {
        public readonly DataType Referent;
        public override bool IsResolved { get; }

        public PointerType(DataType referent)
        {
            Referent = referent;
            IsResolved = referent.IsResolved;
        }

        public override string ToString()
        {
            return "@" + Referent;
        }
    }
}
