using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Constructor { get; }
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public NewObjectExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax constructor,
            [NotNull] FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Constructor = constructor;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"new {Constructor}({Arguments})";
        }
    }
}
