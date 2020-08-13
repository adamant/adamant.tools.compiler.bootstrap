using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

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
            IMetadata? referencedMetadata)
        {
            if (referencedMetadata is null)
                throw new Exception($"Expression doesn't have referenced symbol `{syntax}`");
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax _:
                    // Don't recur into body, we will see those as separate members
                    return;
                case IInvocableNameSyntax syn:
                    WalkChildren(syn);
                    AssertHasReferencedSymbol(syn, syn.ReferencedFunctionMetadata);
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
