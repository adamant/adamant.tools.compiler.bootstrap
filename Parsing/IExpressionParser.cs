using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IExpressionParser
    {
        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression();

        [MustUseReturnValue]
        [NotNull]
        ExpressionSyntax ParseExpression(OperatorPrecedence minPrecedence);

        [MustUseReturnValue]
        [NotNull]
        FixedList<ArgumentSyntax> ParseArguments();
    }
}
