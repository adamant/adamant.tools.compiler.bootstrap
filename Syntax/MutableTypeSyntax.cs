using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MutableTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public MutableTypeSyntax(TextSpan span, [NotNull] ExpressionSyntax referencedType)
            : base(span)
        {
            ReferencedType = referencedType;
        }

        public override string ToString()
        {
            return $"ref {ReferencedType}";
        }
    }
}
