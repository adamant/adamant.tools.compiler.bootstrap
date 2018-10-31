using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class RefType : KnownType
    {
        public bool VariableBinding { get; }
        [CanBeNull] public KnownType ReferencedType { get; }

        public RefType(bool variableBinding, [CanBeNull] KnownType referencedType)
        {
            VariableBinding = variableBinding;
            ReferencedType = referencedType;
        }

        [NotNull]
        public override string ToString()
        {
            return (VariableBinding ? "ref var " : "ref ") + ReferencedType;
        }
    }
}
