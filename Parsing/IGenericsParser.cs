using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IGenericsParser
    {
        [MustUseReturnValue]
        [CanBeNull]
        GenericParametersSyntax AcceptGenericParameters(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);

        [MustUseReturnValue]
        [NotNull]
        SyntaxList<GenericConstraintSyntax> ParseGenericConstraints(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);
    }
}
