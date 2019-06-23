namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class PointerType : ValueType
    {
        public readonly DataType Referent;
        public override bool IsKnown { get; }
        public override ValueSemantics ValueSemantics => ValueSemantics.Copy;

        public PointerType(DataType referent)
        {
            Referent = referent;
            IsKnown = referent.IsKnown;
        }

        public override string ToString()
        {
            return "@" + Referent;
        }
    }
}
