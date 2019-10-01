using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class TreeWalker
    {
        private readonly IDeclarationWalker? declarationWalker;
        private readonly IStatementWalker? statementWalker;
        private readonly ITypeWalker? typeWalker;

        public TreeWalker(
            IDeclarationWalker? declarationWalker,
            IStatementWalker? statementWalker,
            ITypeWalker? typeWalker)
        {
            this.declarationWalker = declarationWalker;
            this.statementWalker = statementWalker;
            this.typeWalker = typeWalker;
        }

        public void Walk(IDeclarationSyntax? declaration)
        {
            if (declaration == null
               || (declarationWalker?.ShouldSkip(declaration) ?? false))
                return;

            switch (declaration)
            {
                default:
                    throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(declaration);
                case IClassDeclarationSyntax classDeclaration:
                    if (declarationWalker?.Enter(classDeclaration) ?? true)
                    {
                        foreach (var member in classDeclaration.Members)
                            Walk(member);
                        declarationWalker?.Exit(classDeclaration);
                    }
                    break;
            }
        }

        public void Walk(ITypeSyntax? type)
        {
            if (typeWalker is null
                || type == null
                || typeWalker.ShouldSkip(type))
                return;

            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case IMutableTypeSyntax mutableType:
                    typeWalker.Enter(mutableType);
                    Walk(mutableType.Referent);
                    typeWalker.Exit(mutableType);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    typeWalker.Enter(referenceLifetimeType);
                    Walk(referenceLifetimeType.ReferentType);
                    typeWalker.Exit(referenceLifetimeType);
                    break;
                case ITypeNameSyntax typeName:
                    typeWalker.Enter(typeName);
                    typeWalker.Exit(typeName);
                    break;
            }
        }
    }
}
