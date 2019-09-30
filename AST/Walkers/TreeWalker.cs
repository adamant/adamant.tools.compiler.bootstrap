using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class TreeWalker
    {
        private readonly IDeclarationWalker? declarationWalker;
        private readonly IStatementWalker? statementWalker;
        private readonly ITypeWalker typeWalker;

        public TreeWalker(
            IDeclarationWalker? declarationWalker,
            IStatementWalker? statementWalker,
            ITypeWalker typeWalker)
        {
            this.declarationWalker = declarationWalker;
            this.statementWalker = statementWalker;
            this.typeWalker = typeWalker;
        }

        public void Walk(IDeclarationSyntax declaration)
        {
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

        public void Walk(TypeSyntax type)
        {
            if (typeWalker is null || typeWalker.ShouldSkip(type))
                return;

            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case MutableTypeSyntax mutableType:
                    typeWalker.Enter(mutableType);
                    Walk(mutableType.Referent);
                    typeWalker.Exit(mutableType);
                    break;
                case ReferenceLifetimeSyntax referenceLifetime:
                    typeWalker.Enter(referenceLifetime);
                    Walk(referenceLifetime.ReferentType);
                    typeWalker.Exit(referenceLifetime);
                    break;
                case SelfTypeSyntax selfType:
                    typeWalker.Enter(selfType);
                    typeWalker.Exit(selfType);
                    break;
                case TypeNameSyntax typeName:
                    typeWalker.Enter(typeName);
                    typeWalker.Exit(typeName);
                    break;
            }
        }
    }
}
