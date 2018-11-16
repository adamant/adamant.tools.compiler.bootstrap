using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class RefType : DataType
    {
        [NotNull] public ObjectType Referent { get; }
        public override bool IsResolved { get; }

        public RefType([NotNull] ObjectType referent)
        {
            Referent = referent;
            IsResolved = referent.IsResolved;
        }

        [NotNull]
        public override string ToString()
        {
            return "ref " + Referent;
        }
    }
}
