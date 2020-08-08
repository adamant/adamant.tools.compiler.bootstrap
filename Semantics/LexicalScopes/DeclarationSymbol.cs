using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class DeclarationSymbol
    {
        public NamespaceName ContainingNamespace { get; }
        public TypeName Name { get; }
        public Promise<Symbol?> Symbol { get; }

        public DeclarationSymbol(INonMemberEntityDeclarationSyntax declaration)
        {
            ContainingNamespace = declaration.ContainingNamespaceName;
            Name = declaration.Name;
            Symbol = declaration.Symbol;
        }

        public DeclarationSymbol(Symbol symbol)
        {
            if (!(symbol.ContainingSymbol is NamespaceOrPackageSymbol containingSymbol))
                throw new ArgumentException("Symbol must be for a non-member entity", nameof(symbol));
            ContainingNamespace = containingSymbol.NamespaceName;
            Name = symbol.Name ?? throw new ArgumentException("Symbol must have a name", nameof(symbol));
            Symbol = Promise.ForValue((Symbol?)symbol);
        }
    }
}
