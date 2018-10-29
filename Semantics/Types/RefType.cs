using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class RefType : DataType
    {
        public bool VariableBinding { get; }
        [CanBeNull] public DataType ReferencedType { get; }

        public RefType(bool variableBinding, [CanBeNull] DataType referencedType)
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
