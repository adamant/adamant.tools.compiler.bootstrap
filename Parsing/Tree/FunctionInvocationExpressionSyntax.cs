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
    internal class FunctionInvocationExpressionSyntax : InvocationExpressionSyntax, IFunctionInvocationExpressionSyntax
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
        public IInvocableNameSyntax FunctionNameSyntax { get; }
        public new Promise<FunctionSymbol?> ReferencedSymbol { get; }

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            Name name,
            IInvocableNameSyntax functionNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, name, arguments, new Promise<FunctionSymbol?>())
        {
            Namespace = NamespaceName.Global;
            FunctionNameSyntax = functionNameSyntax;
            ReferencedSymbol = (Promise<FunctionSymbol?>)base.ReferencedSymbol;
        }

        public IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope()
        {
            if (containingLexicalScope == null)
                throw new InvalidOperationException($"Can't lookup function name without {nameof(ContainingLexicalScope)}");

            // If name is unknown, no symbols
            if (Name is null) return Enumerable.Empty<IPromise<FunctionSymbol>>();

            return containingLexicalScope.Lookup(Name).Select(p => p.As<FunctionSymbol>()).NotNull();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return $"{Name}({string.Join(", ", Arguments)})";
        }
    }
}
