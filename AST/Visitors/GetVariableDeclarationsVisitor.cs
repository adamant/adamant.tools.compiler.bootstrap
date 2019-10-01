using System;
using System.Collections.Generic;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    [Obsolete("Use a walker or switch on types")]
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
