using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public sealed class TypeKindExpressionSyntax : ExpressionSyntax
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
