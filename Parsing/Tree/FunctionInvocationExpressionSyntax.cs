using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IFunctionInvocationExpressionSyntax
    {
        public ICallableNameSyntax FunctionNameSyntax { get; }

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            ICallableNameSyntax functionNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, functionNameSyntax.Name, arguments)
        {
            FunctionNameSyntax = functionNameSyntax;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
