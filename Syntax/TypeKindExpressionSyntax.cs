using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class TypeKindExpressionSyntax : ExpressionSyntax
    {
        public TypeKind TypeKind { get; }

        public TypeKindExpressionSyntax(TextSpan span, TypeKind typeKind)
            : base(span)
        {
            TypeKind = typeKind;
        }

        public override string ToString()
        {
            return $": {TypeKind}";
        }
    }
}
