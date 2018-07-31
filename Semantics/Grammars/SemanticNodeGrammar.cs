using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Grammars
{
    public static class SemanticNodeGrammar
    {
        public static void Build(AttributeGrammar grammar)
        {
            grammar.For(Attribute.SemanticNode)
                .Rule<PackageSyntax>((p, children) =>
                {
                    return new Package(p.Syntax,
                        null,
                        children.Select(c => c.SemanticNode()).Cast<CompilationUnit>(),
                        null);
                });
        }
    }
}
