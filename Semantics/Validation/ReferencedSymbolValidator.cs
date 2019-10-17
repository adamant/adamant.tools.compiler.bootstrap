using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class ReferencedSymbolValidator : SyntaxWalker
    {
        public void Walk(IEnumerable<IEntityDeclarationSyntax> entityDeclaration)
        {
            foreach (var declaration in entityDeclaration)
                WalkNonNull(declaration);
        }

        private static void AssertHasReferencedSymbol(
            ISyntax syntax,
            ISymbol? referencedSymbol)
        {
            if (referencedSymbol == null)
                throw new Exception($"Expression doesn't have referenced symbol `{syntax}`");
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    WalkChildren(memberAccessExpression);
                    AssertHasReferencedSymbol(memberAccessExpression, memberAccessExpression.ReferencedSymbol);
                    return;
                case INameExpressionSyntax nameExpression:
                    WalkChildren(nameExpression);
                    AssertHasReferencedSymbol(nameExpression, nameExpression.ReferencedSymbol);
                    return;
                case ICallableNameSyntax callableName:
                    WalkChildren(callableName);
                    AssertHasReferencedSymbol(callableName, callableName.ReferencedSymbol);
                    return;
                case ITypeNameSyntax typeName:
                    WalkChildren(typeName);
                    AssertHasReferencedSymbol(typeName, typeName.ReferencedSymbol);
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
