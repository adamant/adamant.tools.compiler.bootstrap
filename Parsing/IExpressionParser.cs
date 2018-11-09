using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IExpressionParser
    {
        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);

        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics,
            OperatorPrecedence minPrecedence);

        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);
    }
}
