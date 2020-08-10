using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class SymbolValidator : SyntaxWalker
    {
        private readonly SymbolTree symbolTree;

        public SymbolValidator(SymbolTree symbolTree)
        {
            this.symbolTree = symbolTree;
        }

        public void Walk(IEnumerable<IEntityDeclarationSyntax> entityDeclaration)
        {
            foreach (var declaration in entityDeclaration)
                WalkNonNull(declaration);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax syn:
                    CheckSymbol(syn, syn.Symbol);
                    // Don't recur into body, we will see those as separate members
                    return;
                case IEntityDeclarationSyntax syn:
                    CheckSymbol(syn, syn.Symbol);
                    break;
                    //case ITypeNameSyntax syn:
                    //    CheckReferencedSymbol(syn, syn.ReferencedSymbol);
                    //    break;
            }

            WalkChildren(syntax);
        }

        private void CheckSymbol(ISyntax syntax, IPromise<Symbol?> promise)
        {
            if (promise.State != PromiseState.Fulfilled)
                throw new Exception($"Syntax doesn't have a symbol '{syntax}'");

            if (promise.Result is null)
                throw new Exception($"Syntax has unknown symbol '{syntax}'");

            if (!symbolTree.Contains(promise.Result))
                throw new Exception($"Symbol isn't in the symbol tree '{promise.Result}'");
        }

        private static void CheckReferencedSymbol(ISyntax syntax, IPromise<Symbol?> promise)
        {
            if (promise.State != PromiseState.Fulfilled)
                throw new Exception($"Syntax doesn't have a referenced symbol '{syntax}'");

            if (promise.Result is null)
                throw new Exception($"Syntax has unknown referenced symbol '{syntax}'");
        }
    }
}
