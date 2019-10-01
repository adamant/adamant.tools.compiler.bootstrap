using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssociatedFunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IAssociatedFunctionInvocationExpressionSyntax
    {
        public ITypeSyntax TypeSyntax { get; }
        public INameExpressionSyntax FunctionNameSyntax { get; }

        public AssociatedFunctionInvocationExpressionSyntax(
            TextSpan span,
            ITypeSyntax typeSyntax,
            INameExpressionSyntax functionNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, functionNameSyntax.Name, arguments)
        {
            TypeSyntax = typeSyntax;
            FunctionNameSyntax = functionNameSyntax;
        }

        public override string ToString()
        {
            return $"{TypeSyntax}.{FunctionNameSyntax}({string.Join(", ", Arguments)})";
        }
    }
}
