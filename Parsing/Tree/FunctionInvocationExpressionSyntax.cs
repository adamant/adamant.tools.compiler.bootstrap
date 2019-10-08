using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IFunctionInvocationExpressionSyntax
    {
        public INameExpressionSyntax FunctionNameSyntax { get; }

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            INameExpressionSyntax functionNameSyntax,
            FixedList<ITransferSyntax> arguments)
            : base(span, functionNameSyntax.Name, arguments)
        {
            FunctionNameSyntax = functionNameSyntax;
        }

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
