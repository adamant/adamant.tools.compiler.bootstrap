using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class VariableDeclarationsCollector : SyntaxWalker
    {
        private readonly List<IVariableDeclarationStatementSyntax> variableDeclarations
            = new List<IVariableDeclarationStatementSyntax>();

        public FixedList<IVariableDeclarationStatementSyntax> Declarations =>
            variableDeclarations.ToFixedList();

        public override bool Enter(ISyntax syntax, ISyntaxTraversal traversal)
        {
            switch (syntax)
            {
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    variableDeclarations.Add(variableDeclaration);
                    break;
                case ITypeSyntax _:
                    return false;
                case IDeclarationSyntax _:
                    throw new InvalidOperationException($"Can't get variable declarations of {syntax.GetType().Name}");
            }

            return true;
        }
    }
}
