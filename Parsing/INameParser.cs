using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
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
