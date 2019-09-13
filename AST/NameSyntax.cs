using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IdentifierNameSyntax),
        typeof(GenericNameSyntax))]
    public abstract class NameSyntax : TypeSyntax
    {
        public SimpleName Name { get; }

        private ISymbol referencedSymbol;
        public ISymbol ReferencedSymbol
        {
            get => referencedSymbol;
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedSymbol = value ?? throw new ArgumentException();
            }
        }

        private LexicalScope containingScope;
        public LexicalScope ContainingScope
        {
            get => containingScope;
            set
            {
                if (containingScope != null)
                    throw new InvalidOperationException("Can't set containing scope repeatedly");
                containingScope = value ?? throw new ArgumentException();
            }
        }

        protected NameSyntax(SimpleName name, TextSpan span)
            : base(span)
        {
            Name = name;
        }
    }
}
