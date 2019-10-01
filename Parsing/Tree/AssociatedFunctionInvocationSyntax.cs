using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssociatedFunctionInvocationSyntax : InvocationSyntax, IAssociatedFunctionInvocationSyntax
    {
        public ITypeSyntax TypeSyntax { get; }
        public SimpleName AssociatedFunctionName { get; }

        public AssociatedFunctionInvocationSyntax(
            TextSpan span,
            ITypeSyntax typeSyntax,
            SimpleName associatedFunctionName,
            FixedList<IArgumentSyntax> arguments)
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
