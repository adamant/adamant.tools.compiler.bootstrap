using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class NonMemberEntitySymbol
    {
        public bool InCurrentPackage;
        public NamespaceName ContainingNamespace { get; }
        public TypeName Name { get; }
        public IPromise<Symbol> Symbol { get; }

        public NonMemberEntitySymbol(INonMemberEntityDeclarationSyntax declaration)
        {
            InCurrentPackage = true;
            ContainingNamespace = declaration.ContainingNamespaceName;
            Name = declaration.Name;
            Symbol = declaration.Symbol;
        }

        public NonMemberEntitySymbol(Symbol symbol)
        {
            if (!(symbol.ContainingSymbol is NamespaceOrPackageSymbol
                  || symbol.ContainingSymbol is null))
                throw new ArgumentException("Symbol must be for a non-member entity", nameof(symbol));
            var containingSymbol = symbol.ContainingSymbol as NamespaceOrPackageSymbol;
            InCurrentPackage = false;
            ContainingNamespace = containingSymbol?.NamespaceName ?? NamespaceName.Global;
            Name = symbol.Name ?? throw new ArgumentException("Symbol must have a name", nameof(symbol));
            Symbol = Promise.ForValue(symbol);
        }
    }
}
