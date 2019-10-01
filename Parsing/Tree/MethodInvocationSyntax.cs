using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MethodInvocationSyntax : InvocationSyntax, IMethodInvocationSyntax
    {
        private IExpressionSyntax target;
        public ref IExpressionSyntax Target => ref target;

        public INameSyntax MethodNameSyntax { get; }

        public MethodInvocationSyntax(
            TextSpan span,
            IExpressionSyntax target,
            INameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, methodNameSyntax.Name, arguments)
        {
            this.target = target;
            MethodNameSyntax = methodNameSyntax;
        }

        public override string ToString()
        {
            return $"{Target}.{FunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
