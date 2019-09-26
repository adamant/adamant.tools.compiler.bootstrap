using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// The potentially qualified name of a type (i.e. `foo.bar.Baz`)
    /// </summary>
    public class TypeNameSyntax : TypeSyntax
    {
        public Name Name { get; }

        private ISymbol? referencedSymbol;
        public ISymbol? ReferencedSymbol
        {
            get => referencedSymbol;
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedSymbol = value ?? throw new ArgumentException();
            }
        }

        private LexicalScope? containingScope;
        public LexicalScope? ContainingScope
        {
            get => containingScope;
            set
            {
                if (containingScope != null)
                    throw new InvalidOperationException("Can't set containing scope repeatedly");
                containingScope = value ?? throw new ArgumentException();
            }
        }

        public TypeNameSyntax(TextSpan span, Name name)
            : base(span)
        {
            Name = name;
        }

        public FixedList<ISymbol> LookupInContainingScope()
        {
            if (ContainingScope != null)
                return ContainingScope.LookupQualified(Name);

            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
