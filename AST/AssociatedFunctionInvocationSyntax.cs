using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AssociatedFunctionInvocationSyntax : InvocationSyntax
    {
        public TypeSyntax TypeSyntax { get; }
        public SimpleName AssociatedFunctionName { get; }

        public AssociatedFunctionInvocationSyntax(
            TextSpan span,
            TypeSyntax typeSyntax,
            SimpleName associatedFunctionName,
            FixedList<ArgumentSyntax> arguments)
            : base(span, associatedFunctionName, arguments)
        {
            TypeSyntax = typeSyntax;
            AssociatedFunctionName = associatedFunctionName;
        }

        public override string ToString()
        {
            return $"{TypeSyntax}.{AssociatedFunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
