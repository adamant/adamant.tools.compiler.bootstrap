using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MethodInvocationExpressionSyntax : InvocationExpressionSyntax, IMethodInvocationExpressionSyntax
    {
        private IExpressionSyntax target;
        public ref IExpressionSyntax Target => ref target;

        public ICallableNameSyntax MethodNameSyntax { get; }

        public MethodInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax target,
            ICallableNameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, methodNameSyntax.Name, arguments)
        {
            this.target = target;
            MethodNameSyntax = methodNameSyntax;
        }

        public override string ToString()
        {
            return $"{Target}.{FullName}({string.Join(", ", Arguments)})";
        }
    }
}
