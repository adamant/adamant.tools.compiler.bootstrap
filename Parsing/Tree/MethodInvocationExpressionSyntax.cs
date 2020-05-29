using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MethodInvocationExpressionSyntax : InvocationExpressionSyntax, IMethodInvocationExpressionSyntax
    {
        private IExpressionSyntax contextExpression;
        public ref IExpressionSyntax ContextExpression => ref contextExpression;

        public ICallableNameSyntax MethodNameSyntax { get; }

        public MethodInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax contextExpression,
            ICallableNameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, methodNameSyntax.Name, arguments)
        {
            this.contextExpression = contextExpression;
            MethodNameSyntax = methodNameSyntax;
        }

        public override string ToString()
        {
            return $"{ContextExpression.ToGroupedString()}.{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
