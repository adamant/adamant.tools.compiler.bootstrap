using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class FloatingPointType : NumericType
    {
        [NotNull] public static readonly FloatingPointType Float32 = new FloatingPointType("float32", 32);
        [NotNull] public static readonly FloatingPointType Float = new FloatingPointType("float", 64);

        public readonly uint Bits;
        public override bool IsResolved => true;

        private FloatingPointType([NotNull] string name, uint bits)
            : base(name)
        {
            Bits = bits;
        }
    }
}
