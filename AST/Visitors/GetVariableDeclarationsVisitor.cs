using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetVariableDeclarationsVisitor : ExpressionVisitor<Void>
    {
        public IReadOnlyList<VariableDeclarationStatementSyntax> VariableDeclarations => variableDeclarations;
        private readonly List<VariableDeclarationStatementSyntax> variableDeclarations = new List<VariableDeclarationStatementSyntax>();

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            variableDeclarations.Add(variableDeclaration);
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
        }
    }
}
