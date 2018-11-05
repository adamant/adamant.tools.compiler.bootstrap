using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public interface IDeclarationAnalysis : ISymbol
    {
        [NotNull] QualifiedName Name { get; }
        [CanBeNull] DataType Type { get; }
    }
}
