namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxNode
    {
        public SyntaxList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(SyntaxList<CompilationUnitSyntax> compilationUnits)
        {
            CompilationUnits = compilationUnits;
        }
    }
}
