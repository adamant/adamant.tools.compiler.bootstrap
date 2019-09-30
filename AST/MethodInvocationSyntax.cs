using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MethodInvocationSyntax : InvocationSyntax
    {
        public ExpressionSyntax Target;
        public NameSyntax MethodNameSyntax { get; }

        public MethodInvocationSyntax(
            TextSpan span,
            ExpressionSyntax target,
            NameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, methodNameSyntax.Name, arguments)
        {
            Target = target;
            MethodNameSyntax = methodNameSyntax;
        }

        public override string ToString()
        {
            return $"{Target}.{FunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
