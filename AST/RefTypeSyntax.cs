using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class RefTypeSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferencedType { get; }

        public RefTypeSyntax(TextSpan span, ExpressionSyntax referencedType)
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
