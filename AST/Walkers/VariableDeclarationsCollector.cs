using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class VariableDeclarationsCollector : StatementWalker
    {
        private readonly List<IVariableDeclarationStatementSyntax> variableDeclarations
            = new List<IVariableDeclarationStatementSyntax>();

        public FixedList<IVariableDeclarationStatementSyntax> Declarations =>
            variableDeclarations.ToFixedList();

        public override void Enter(IVariableDeclarationStatementSyntax variableDeclaration)
        {
            variableDeclarations.Add(variableDeclaration);
        }

        public override void Enter(IExpressionStatementSyntax expressionStatement) { }

        public override void Enter(IResultStatementSyntax resultStatement) { }
    }
}
