using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MethodInvocationSyntax : InvocationSyntax
    {
        public ExpressionSyntax Target;
        public SimpleName MethodName { get; }

        public MethodInvocationSyntax(
            TextSpan span,
            ExpressionSyntax target,
            SimpleName methodName,
            FixedList<ArgumentSyntax> arguments)
            : base(span, methodName, arguments)
        {
            Target = target;
            MethodName = methodName;
        }

        public override string ToString()
        {
            return $"{Target}.{MethodName}({string.Join(", ", Arguments)})";
        }
    }
}
