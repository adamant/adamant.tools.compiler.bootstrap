using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ArgumentSyntax : NonTerminal
    {
        [CanBeNull] public IParamsKeywordToken ParamsKeyword { get; }
        [CanBeNull] public ExpressionSyntax Value { get; }

        public ArgumentSyntax(
            [CanBeNull] IParamsKeywordToken paramsKeyword,
            [CanBeNull] ExpressionSyntax value)
        {
            ParamsKeyword = paramsKeyword;
            Value = value;
        }
    }
}
