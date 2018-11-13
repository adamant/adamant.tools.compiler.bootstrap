using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericParameterSyntax : NonTerminal
    {
        public bool IsParams { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public ExpressionSyntax Type { get; }

        public GenericParameterSyntax(
            bool isParams,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] ExpressionSyntax type)
        {
            Name = name;
            Type = type;
            IsParams = isParams;
        }
    }
}
