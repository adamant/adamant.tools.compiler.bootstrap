using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
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

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
