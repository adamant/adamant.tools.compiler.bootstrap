using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ThrowEffectEntrySyntax : SyntaxNode
    {
        [CanBeNull] public ParamsKeywordToken ParamsKeyword { get; }
        [NotNull] public ExpressionSyntax ExceptionType { get; }

        public ThrowEffectEntrySyntax(
            [CanBeNull] ParamsKeywordToken paramsKeyword,
            [NotNull] ExpressionSyntax exceptionType)
        {
            Requires.NotNull(nameof(exceptionType), exceptionType);
            ParamsKeyword = paramsKeyword;
            ExceptionType = exceptionType;
        }
    }
}
