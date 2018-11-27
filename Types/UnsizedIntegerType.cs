using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class UnsizedIntegerType : IntegerType
    {
        [NotNull] public static readonly UnsizedIntegerType Size = new UnsizedIntegerType("size", false);
        [NotNull] public static readonly UnsizedIntegerType Offset = new UnsizedIntegerType("offset", true);

        public readonly bool IsSigned;
        public override bool IsResolved => true;

        private UnsizedIntegerType([NotNull] string name, bool signed)
            : base(name)
        {
            IsSigned = signed;
        }
    }
}
