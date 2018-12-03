using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class MemberAccessValue : Value
    {
        [NotNull] public Operand Expression { get; }
        [NotNull] public Name Member { get; }

        public MemberAccessValue([NotNull] Operand expression, [NotNull] Name member)
        {
            Expression = expression;
            Member = member;
        }

        public override string ToString()
        {
            return $"{Expression}.{Member}";
        }
    }
}
