namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class PointerType : ValueType
    {
        public readonly DataType Referent;
        public override bool IsResolved { get; }
        public override ValueSemantics Semantics => ValueSemantics.Copy;

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
