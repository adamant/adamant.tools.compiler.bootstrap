using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PointerType : KnownType
    {
        [NotNull] public readonly DataType ReferencedType;

        public PointerType([NotNull] DataType referencedType)
        {
            Requires.NotNull(nameof(referencedType), referencedType);
            ReferencedType = referencedType;
        }

        public override string ToString()
        {
            return "@" + ReferencedType;
        }
    }
}
