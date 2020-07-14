using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.FST.Walkers
{
    public static class WalkExtensions
    {
        public static FixedList<IBindingSymbol> GetAllVariableDeclarations(
            this IBodySyntax body)
        {
            var collector = new VariableDeclarationsCollector();
            collector.Walk(body);
            return collector.Symbols;
        }
    }
}
