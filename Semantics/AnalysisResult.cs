using JetBrains.Annotations;
using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public struct AnalysisResult
    {
        [NotNull]
        public readonly Package Package;

        [NotNull]
        public readonly ILPackage IL;

        public AnalysisResult([NotNull] Package package, [NotNull] ILPackage il)
        {
            Package = package;
            IL = il;
        }
    }
}
