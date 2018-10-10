using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxNode
    {
        [NotNull] public string Name { get; }
        [NotNull] public SyntaxList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(
            [NotNull] string name,
            [NotNull] SyntaxList<CompilationUnitSyntax> compilationUnits)
        {
            Name = name;
            CompilationUnits = compilationUnits;
        }
    }
}
