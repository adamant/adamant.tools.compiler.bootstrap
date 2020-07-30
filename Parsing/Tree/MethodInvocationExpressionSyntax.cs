using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

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

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{ContextExpression.ToGroupedString(ExpressionPrecedence)}.{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
