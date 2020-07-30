using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class CallableNameSyntax : ICallableNameSyntax
    {
        public TextSpan Span { get; }
        public Name Name { get; }

        private IFunctionSymbol? referencedSymbol;

        [DisallowNull]
        public IFunctionSymbol? ReferencedSymbol
        {
            get => referencedSymbol;
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private readonly LexicalScope containingScope;

        [DisallowNull]
        public LexicalScope? ContainingScope
        {
            get => containingScope;
            set => throw new InvalidOperationException("Can't set containing scope repeatedly");
        }

        public CallableNameSyntax(TextSpan span, Name name, LexicalScope containingScope)
        {
            Span = span;
            Name = name;
            this.containingScope = containingScope;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
