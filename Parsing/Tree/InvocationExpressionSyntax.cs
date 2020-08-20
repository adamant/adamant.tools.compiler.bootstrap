using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocationExpressionSyntax : ExpressionSyntax, IInvocationExpressionSyntax
    {
        public Name InvokedName { get; }
        public TextSpan InvokedNameSpan { get; }
        public FixedList<IArgumentSyntax> Arguments { [DebuggerStepThrough] get; }
        public IPromise<InvocableSymbol?> ReferencedSymbol { get; }

        private protected InvocationExpressionSyntax(
            TextSpan span,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments,
            IPromise<InvocableSymbol?> referencedSymbol)
            : base(span)
        {
            InvokedName = invokedName;
            Arguments = arguments;
            ReferencedSymbol = referencedSymbol;
            InvokedNameSpan = invokedNameSpan;
        }
    }
}
