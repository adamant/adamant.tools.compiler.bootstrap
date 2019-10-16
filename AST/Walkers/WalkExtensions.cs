using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class WalkExtensions
    {
        public static FixedList<IVariableDeclarationStatementSyntax> GetAllVariableDeclarations(
            this IBodySyntax body)
        {
            var collector = new VariableDeclarationsCollector();
            collector.Walk(body);
            return collector.Declarations;
        }
    }
}
