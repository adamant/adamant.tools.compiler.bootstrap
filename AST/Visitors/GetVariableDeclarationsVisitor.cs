
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetVariableDeclarationsVisitor : ExpressionVisitor<Void, IEnumerable<VariableDeclarationStatementSyntax>>
    {
        #region Singleton
        [NotNull] public static readonly GetVariableDeclarationsVisitor Instance = new GetVariableDeclarationsVisitor();

        private GetVariableDeclarationsVisitor() { }
        #endregion

        [NotNull]
        public override IEnumerable<VariableDeclarationStatementSyntax> DefaultResult(
            Void args)
        {
            return Enumerable.Empty<VariableDeclarationStatementSyntax>();
        }

        [NotNull]
        public override IEnumerable<VariableDeclarationStatementSyntax> CombineResults(
            Void args,
            [NotNull] params IEnumerable<VariableDeclarationStatementSyntax>[] results)
        {
            return results.SelectMany(r => r);
        }

        [NotNull]
        public override IEnumerable<VariableDeclarationStatementSyntax> VisitVariableDeclarationStatement(
            [NotNull] VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            return variableDeclaration.Yield();
        }
    }
}
