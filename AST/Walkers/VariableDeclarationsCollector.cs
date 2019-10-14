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

        public override bool Enter(FixedList<IStatementSyntax> statements, ITreeWalker walker)
        {
            return true;
        }

        public override bool Enter(IStatementSyntax statement, ITreeWalker walker)
        {
            if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                variableDeclarations.Add(variableDeclaration);
            return true;
        }
    }
}
