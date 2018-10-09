using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public struct AnalysisResult
    {
        [NotNull] public readonly Package Package;
        [NotNull] public readonly PackageIL IL;

        public AnalysisResult([NotNull] Package package, [NotNull] PackageIL il)
        {
            Requires.NotNull(nameof(package), package);
            Requires.NotNull(nameof(il), il);
            Package = package;
            IL = il;
        }
    }
}
