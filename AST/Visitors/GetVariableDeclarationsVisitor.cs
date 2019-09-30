using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetVariableDeclarationsVisitor : ExpressionVisitor<Void>
    {
        public IReadOnlyList<IVariableDeclarationStatementSyntax> VariableDeclarations => variableDeclarations;
        private readonly List<IVariableDeclarationStatementSyntax> variableDeclarations = new List<IVariableDeclarationStatementSyntax>();

        public override void VisitVariableDeclarationStatement(
            IVariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            variableDeclarations.Add(variableDeclaration);
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
        }
    }
}
