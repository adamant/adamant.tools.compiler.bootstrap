using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
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
