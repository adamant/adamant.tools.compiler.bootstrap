using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface INameParser
    {
        [MustUseReturnValue]
        [NotNull]
        NameSyntax ParseName(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);
    }
}
