using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class UnsizedIntegerType : IntegerType
    {
        [NotNull] internal static readonly UnsizedIntegerType Size = new UnsizedIntegerType("size", false);
        [NotNull] internal static readonly UnsizedIntegerType Offset = new UnsizedIntegerType("offset", true);

        public readonly bool IsSigned;
        public override bool IsResolved => true;

        private UnsizedIntegerType([NotNull] string name, bool signed)
            : base(name)
        {
            IsSigned = signed;
        }
    }
}
