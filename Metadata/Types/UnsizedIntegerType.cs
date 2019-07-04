namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class UnsizedIntegerType : IntegerType
    {
        internal new static readonly UnsizedIntegerType Size = new UnsizedIntegerType("size", false);
        internal new static readonly UnsizedIntegerType Offset = new UnsizedIntegerType("offset", true);

        public readonly bool IsSigned;
        public override bool IsKnown => true;

        private UnsizedIntegerType(string name, bool signed)
            : base(name)
        {
            IsSigned = signed;
        }
    }
}
