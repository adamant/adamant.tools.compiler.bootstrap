using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IExpressionParser
    {
        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics);

        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics,
            OperatorPrecedence minPrecedence);

        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics);
    }
}
