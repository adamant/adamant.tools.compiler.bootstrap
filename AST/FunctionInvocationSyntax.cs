using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FunctionInvocationSyntax : InvocationSyntax, IFunctionInvocationSyntax
    {
        public INameSyntax FunctionNameSyntax { get; }

        public FunctionInvocationSyntax(
            TextSpan span,
            INameSyntax functionNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, functionNameSyntax.Name, arguments)
        {
            FunctionNameSyntax = functionNameSyntax;
        }

        public override string ToString()
        {
            return $"{FunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
