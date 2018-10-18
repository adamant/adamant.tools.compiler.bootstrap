using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public INewKeywordToken NewKeyword { get; }
        [NotNull] public TypeSyntax Type { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        [NotNull] public ICloseParenToken CloseParen { get; }

        public NewObjectExpressionSyntax(
            [NotNull] INewKeywordToken newKeyword,
            [NotNull] TypeSyntax type,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> arguments,
            [NotNull] ICloseParenToken closeParen)
        {
            Requires.NotNull(nameof(arguments), arguments);
            NewKeyword = newKeyword;
            Type = type;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
