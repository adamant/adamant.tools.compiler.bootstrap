using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocationSyntax : ExpressionSyntax, IInvocationSyntax
    {
        public Name FunctionName { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }

        private protected InvocationSyntax(
            TextSpan span,
            Name functionName,
            FixedList<IArgumentSyntax> arguments)
            : base(span)
        {
            Arguments = arguments;
            FunctionName = functionName;
        }
    }
}
