using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ArgumentSyntax : SyntaxNode
    {
        [CanBeNull] public ParamsKeywordToken ParamsKeyword { get; }
        [CanBeNull] public ExpressionSyntax Value { get; }

        public ArgumentSyntax(
            [CanBeNull] ParamsKeywordToken paramsKeyword,
            [CanBeNull] ExpressionSyntax value)
        {
            ParamsKeyword = paramsKeyword;
            Value = value;
        }
    }
}
