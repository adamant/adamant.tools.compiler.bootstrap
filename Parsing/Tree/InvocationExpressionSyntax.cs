using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocationExpressionSyntax : ExpressionSyntax, IInvocationExpressionSyntax
    {
        public Name FullName { [DebuggerStepThrough] get; }
        public FixedList<IArgumentSyntax> Arguments { [DebuggerStepThrough] get; }

        private protected InvocationExpressionSyntax(
            TextSpan span,
            Name functionName,
            FixedList<IArgumentSyntax> arguments)
            : base(span)
        {
            Arguments = arguments;
            FullName = functionName;
        }
    }
}
