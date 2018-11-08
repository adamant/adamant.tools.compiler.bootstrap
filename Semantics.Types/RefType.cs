using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class RefType : DataType
    {
        public bool VariableBinding { get; }
        [NotNull] public ObjectType Referent { get; }
        public override bool IsResolved { get; }

        public RefType(bool variableBinding, [NotNull] ObjectType referent)
        {
            Requires.NotNull(nameof(referent), referent);
            VariableBinding = variableBinding;
            Referent = referent;
            IsResolved = referent.IsResolved;
        }

        [NotNull]
        public override string ToString()
        {
            return (VariableBinding ? "ref var " : "ref ") + Referent;
        }
    }
}
