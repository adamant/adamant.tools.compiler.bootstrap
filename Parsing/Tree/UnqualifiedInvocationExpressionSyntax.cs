using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class UnqualifiedInvocationExpressionSyntax : InvocationExpressionSyntax, IUnqualifiedInvocationExpressionSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        public NamespaceName Namespace { get; }
        public new Promise<FunctionSymbol?> ReferencedSymbol { get; }

        public UnqualifiedInvocationExpressionSyntax(
            TextSpan span,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments)
            : base(span, invokedName, invokedNameSpan, arguments, new Promise<FunctionSymbol?>())
        {
            Namespace = NamespaceName.Global;
            ReferencedSymbol = (Promise<FunctionSymbol?>)base.ReferencedSymbol;
        }

        public IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope()
        {
            if (containingLexicalScope == null)
                throw new InvalidOperationException($"Can't lookup function name without {nameof(ContainingLexicalScope)}");

            // If name is unknown, no symbols
            if (InvokedName is null) return Enumerable.Empty<IPromise<FunctionSymbol>>();

            return containingLexicalScope.Lookup(InvokedName).Select(p => p.As<FunctionSymbol>()).WhereNotNull();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return $"{InvokedName}({string.Join(", ", Arguments)})";
        }
    }
}
