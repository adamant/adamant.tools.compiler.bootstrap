using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze(PackageSyntax packageSyntax)
        {
            var analysis = new SemanticAnalysis(packageSyntax);
            var package = analysis.Package;
            return package;
        }
    }
}
