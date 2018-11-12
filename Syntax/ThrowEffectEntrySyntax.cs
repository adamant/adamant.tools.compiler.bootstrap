using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ThrowEffectEntrySyntax : NonTerminal
    {
        public bool IsParams { get; }
        [NotNull] public ExpressionSyntax ExceptionType { get; }

        public ThrowEffectEntrySyntax(
            bool isParams,
            [NotNull] ExpressionSyntax exceptionType)
        {
            Requires.NotNull(nameof(exceptionType), exceptionType);
            IsParams = isParams;
            ExceptionType = exceptionType;
        }
    }
}
