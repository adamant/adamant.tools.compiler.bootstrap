using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
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
