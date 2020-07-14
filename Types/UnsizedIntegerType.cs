namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// Integer types whose exact bit length is architecture dependent
    /// </summary>
    // TODO Sized/Unsized are the wrong terms. They should mean as in Rust that the compiler can't assign a size
    public sealed class UnsizedIntegerType : IntegerType
    {
        internal new static readonly UnsizedIntegerType Size = new UnsizedIntegerType("size", false);
        internal new static readonly UnsizedIntegerType Offset = new UnsizedIntegerType("offset", true);

        public bool IsSigned { get; }
        public override bool IsKnown => true;

        private UnsizedIntegerType(string name, bool signed)
            : base(name)
        {
            IsSigned = signed;
        }
    }
}
