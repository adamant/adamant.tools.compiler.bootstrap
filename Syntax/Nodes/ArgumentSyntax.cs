using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ArgumentSyntax : SyntaxNode
    {
        [CanBeNull] public ParamsKeywordToken ParamsKeyword { get; }
        [NotNull] public ExpressionSyntax Value { get; }

        public ArgumentSyntax(
            [CanBeNull] ParamsKeywordToken paramsKeyword,
            [NotNull] ExpressionSyntax value)
        {
            Requires.NotNull(nameof(value), value);
            ParamsKeyword = paramsKeyword;
            Value = value;
        }
    }
}
