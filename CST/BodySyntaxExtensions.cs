using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public static class BodySyntaxExtensions
    {
        public static FixedList<IBindingMetadata> GetAllVariableDeclarations(
            this IBodySyntax body)
        {
            var collector = new VariableDeclarationsCollector();
            collector.Walk(body);
            return collector.Metadata;
        }
    }
}
