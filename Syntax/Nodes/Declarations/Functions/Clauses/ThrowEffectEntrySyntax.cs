using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Clauses
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
