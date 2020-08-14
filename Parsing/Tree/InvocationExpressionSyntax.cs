using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocationExpressionSyntax : ExpressionSyntax, IInvocationExpressionSyntax
    {
        public Name Name { get; }
        public FixedList<IArgumentSyntax> Arguments { [DebuggerStepThrough] get; }

        private protected InvocationExpressionSyntax(
            TextSpan span,
            Name name,
            FixedList<IArgumentSyntax> arguments)
            : base(span)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
