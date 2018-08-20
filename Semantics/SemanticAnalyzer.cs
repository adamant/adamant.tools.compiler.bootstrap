using Adamant.Tools.Compiler.Bootstrap.Semantics.IL;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public AnalysisResult Analyze(PackageSyntax packageSyntax)
        {
            var analysis = new SemanticAnalysis(packageSyntax);
            var package = analysis.Package;
            var ilPackage = new IntermediateLanguageGenerator().Convert(package);
            // TODO do borrow checking
            return new AnalysisResult(package, ilPackage);
        }
    }
}
