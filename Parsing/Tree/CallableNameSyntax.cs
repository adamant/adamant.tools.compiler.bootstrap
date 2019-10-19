using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class CallableNameSyntax : Syntax, ICallableNameSyntax
    {
        public SimpleName Name { get; }

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

        public CallableNameSyntax(TextSpan span, SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public FixedList<ISymbol> LookupInContainingScope()
        {
            if (ContainingScope != null) return ContainingScope.Lookup(Name);
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
