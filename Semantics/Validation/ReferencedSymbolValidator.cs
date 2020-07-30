using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
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
            if (referencedSymbol is null)
                throw new Exception($"Expression doesn't have referenced symbol `{syntax}`");
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax _:
                    // Don't recur into body, we will see those as separate members
                    return;
                case IFieldAccessExpressionSyntax memberAccessExpression:
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
                case IMoveExpressionSyntax move:
                    WalkChildren(move);
                    AssertHasReferencedSymbol(move, move.MovedSymbol);
                    return;
                case IBorrowExpressionSyntax borrow:
                    WalkChildren(borrow);
                    AssertHasReferencedSymbol(borrow, borrow.BorrowedSymbol);
                    return;
                case IShareExpressionSyntax share:
                    WalkChildren(share);
                    AssertHasReferencedSymbol(share, share.SharedSymbol);
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
