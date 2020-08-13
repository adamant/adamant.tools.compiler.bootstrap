using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MethodInvocationExpressionSyntax : InvocationExpressionSyntax, IMethodInvocationExpressionSyntax
    {
        private IExpressionSyntax contextExpression;
        public ref IExpressionSyntax ContextExpression => ref contextExpression;

        public IInvocableNameSyntax MethodNameSyntax { get; }
        public Promise<MethodSymbol?> ReferencedSymbol { get; } = new Promise<MethodSymbol?>();

        public MethodInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax contextExpression,
            Name name,
            IInvocableNameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, name, methodNameSyntax.Name, arguments)
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
