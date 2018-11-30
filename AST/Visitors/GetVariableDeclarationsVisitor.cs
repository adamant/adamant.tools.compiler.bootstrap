using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetVariableDeclarationsVisitor : ExpressionVisitor<Void>
    {
        [NotNull] public IReadOnlyList<VariableDeclarationStatementSyntax> VariableDeclarations => variableDeclarations;
        [NotNull] private readonly List<VariableDeclarationStatementSyntax> variableDeclarations = new List<VariableDeclarationStatementSyntax>();

        [NotNull]
        public override void VisitVariableDeclarationStatement(
            [NotNull] VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            variableDeclarations.Add(variableDeclaration);
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
        }
    }
}
