using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// The potentially qualified name of a type (i.e. `foo.bar.Baz`)
    /// </summary>
    internal class TypeNameSyntax : TypeSyntax, ITypeNameSyntax
    {
        public Name Name { get; }

        private ISymbol? referencedSymbol;
        [DisallowNull]
        public ISymbol? ReferencedSymbol
        {
            get => referencedSymbol;
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private LexicalScope? containingScope;
        [DisallowNull]
        public LexicalScope? ContainingScope
        {
            get => containingScope;
            set
            {
                if (containingScope != null)
                    throw new InvalidOperationException("Can't set containing scope repeatedly");
                containingScope = value ?? throw new ArgumentNullException(nameof(value));
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
                return ContainingScope.Lookup(Name);

            throw new InvalidOperationException("Can't lookup type name without containing scope");
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
