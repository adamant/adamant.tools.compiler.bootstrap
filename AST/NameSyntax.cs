using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// A name of a variable or namespace
    /// </summary>
    public class NameSyntax : ExpressionSyntax
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

        public NameSyntax(TextSpan span, SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public FixedList<ISymbol> LookupInContainingScope()
        {
            return ContainingScope.Lookup(Name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
