using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class WalkExtensions
    {
        public static void Walk(this ITypeWalker walker, ITypeSyntax? type)
        {
            var treeWalker = new TreeWalker(null, walker, null, null);
            treeWalker.Walk(type);
        }

        public static void Walk(this IStatementWalker walker, FixedList<IStatementSyntax>? statements)
        {
            var treeWalker = new TreeWalker(null, walker as ITypeWalker, walker, walker as IExpressionWalker);
            treeWalker.Walk(statements);
        }

        public static void Walk(this IStatementWalker walker, IStatementSyntax? statement)
        {
            var treeWalker = new TreeWalker(null, walker as ITypeWalker, walker, walker as IExpressionWalker);
            treeWalker.Walk(statement);
        }

        public static void Walk(this IExpressionWalker walker, IExpressionSyntax? expression)
        {
            var treeWalker = new TreeWalker(null, walker as ITypeWalker, walker as IStatementWalker, walker);
            treeWalker.Walk(expression);
        }

        public static FixedList<IVariableDeclarationStatementSyntax> GetAllVariableDeclarations(
            this IEnumerable<IStatementSyntax> statements)
        {
            var collector = new VariableDeclarationsCollector();
            var treeWalker = new TreeWalker(null, null, collector, null);
            foreach (var statement in statements)
                treeWalker.Walk(statement);

            return collector.Declarations;
        }
    }
}
