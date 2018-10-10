using Adamant.Tools.Compiler.Bootstrap.Semantics.IL;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public AnalysisResult Analyze([NotNull] PackageSyntax packageSyntax)
        {
            var analysis = new SemanticAnalysis(packageSyntax);
            var package = analysis.Package;
            var ilPackage = new IntermediateLanguageGenerator().Convert(package);
            var borrowChecker = new BorrowChecker.BorrowChecker();
            borrowChecker.Check(ilPackage);
            return new AnalysisResult(package, ilPackage);
        }
    }
}
