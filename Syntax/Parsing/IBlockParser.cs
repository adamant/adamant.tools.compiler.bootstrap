using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface IBlockParser
    {
        [MustUseReturnValue]
        [CanBeNull]
        BlockSyntax AcceptBlock(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);

        [NotNull]
        [MustUseReturnValue]
        BlockSyntax ParseBlock(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);
    }
}
