using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxNode
    {
        [NotNull] public SyntaxList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax([NotNull] SyntaxList<CompilationUnitSyntax> compilationUnits)
        {
            CompilationUnits = compilationUnits;
        }
    }
}
