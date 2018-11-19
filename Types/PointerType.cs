using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class PointerType : DataType
    {
        [NotNull] public readonly DataType Referent;
        public override bool IsResolved { get; }

        public PointerType([NotNull] DataType referent)
        {
            Requires.NotNull(nameof(referent), referent);
            Referent = referent;
            IsResolved = referent.IsResolved;
        }

        public override string ToString()
        {
            return "@" + Referent;
        }
    }
}
