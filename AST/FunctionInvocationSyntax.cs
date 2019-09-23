using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FunctionInvocationSyntax : InvocationSyntax
    {
        internal FunctionInvocationSyntax(
            TextSpan span,
            Name functionName,
            FixedList<ArgumentSyntax> arguments)
            : base(span, functionName, arguments)
        {
        }

        public override string ToString()
        {
            return $"{FunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
