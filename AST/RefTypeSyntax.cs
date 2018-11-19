using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class RefTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public RefTypeSyntax(TextSpan span, [NotNull] ExpressionSyntax referencedType)
            : base(span)
        {
            ReferencedType = referencedType;
        }

        public override string ToString()
        {
            return $"ref var {ReferencedType}";
        }
    }
}
