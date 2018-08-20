using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public struct AnalysisResult
    {
        public readonly Package Package;
        public readonly ILPackage IL;

        public AnalysisResult(Package package, ILPackage il)
        {
            Package = package;
            IL = il;
        }
    }
}
