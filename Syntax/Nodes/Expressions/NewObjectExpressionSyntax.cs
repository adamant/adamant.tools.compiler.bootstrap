using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull]
        public NewKeywordToken NewKeyword { get; }

        public TypeSyntax Type { get; }

        [CanBeNull]
        public OpenParenToken OpenParen { get; }

        [NotNull]
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }

        [CanBeNull]
        public CloseParenToken CloseParen { get; }

        public NewObjectExpressionSyntax(
            [CanBeNull] NewKeywordToken newKeyword,
            TypeSyntax type,
            [CanBeNull] OpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> arguments,
            [CanBeNull] CloseParenToken closeParen)
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
