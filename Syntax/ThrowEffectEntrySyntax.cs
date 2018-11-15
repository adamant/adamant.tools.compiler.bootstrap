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
            IsParams = isParams;
            ExceptionType = exceptionType;
        }
    }
}
