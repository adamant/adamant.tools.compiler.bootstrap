namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class FloatingPointType : NumericType
    {
        internal static new readonly FloatingPointType Float32 = new FloatingPointType("float32", 32);
        internal static new readonly FloatingPointType Float = new FloatingPointType("float", 64);

        public readonly uint Bits;
        public override bool IsKnown => true;

        private FloatingPointType(string name, uint bits)
            : base(name)
        {
            Bits = bits;
        }
    }
}
