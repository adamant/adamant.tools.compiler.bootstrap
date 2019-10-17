using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class AssociatedFunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IAssociatedFunctionInvocationExpressionSyntax
    {
        public ITypeSyntax TypeSyntax { get; }
        public ICallableNameSyntax FunctionNameSyntax { get; }

        public AssociatedFunctionInvocationExpressionSyntax(
            TextSpan span,
            ITypeSyntax typeSyntax,
            ICallableNameSyntax functionNameSyntax,
            FixedList<ITransferSyntax> arguments)
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
